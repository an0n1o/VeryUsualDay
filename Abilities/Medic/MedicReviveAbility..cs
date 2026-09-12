using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CustomPlayerEffects;

using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;

using MEC;

using PlayerRoles;

using UnityEngine;

namespace VeryUsualDay.Abilities.Medic
{
    public class MedicReviveAbility : BaseAbility
    {
        private const float ReviveWindow = 60f;


        private const float ExpiredInterruptedCleanupDelay = 25f;

        private const float DisconnectedCleanupDelay = 5f;

        private const float MaxReviveDistance = 1f;
        private const float MinLookDot = 0.8f;

        private const int BarLength = 21;
        private const float MarkerStepTime = 0.1f;

        private const int FreshSuccessZoneWidth = 4;
        private const int MediumSuccessZoneWidth = 3;
        private const int OldSuccessZoneWidth = 2;

        private static readonly Dictionary<int, DeathSnapshot> PendingDeaths =
            new Dictionary<int, DeathSnapshot>();

        private static readonly Dictionary<int, DeathRecord> DeathRecords =
            new Dictionary<int, DeathRecord>();

        private static readonly Dictionary<int, ReviveSession> Sessions =
            new Dictionary<int, ReviveSession>();

        private static readonly HashSet<int> RevivedThisLife =
            new HashSet<int>();

        private static readonly HashSet<int> WaitingForNewLife =
            new HashSet<int>();

        private static readonly HashSet<int> DeathLobbyTransitions =
            new HashSet<int>();

        private static readonly HashSet<int> RevivalRoleChanges =
            new HashSet<int>();

        public MedicReviveAbility() : base(
            4,
            "Медик: Реанимация",
            KeyCode.H,
            "Позволяет медику реанимировать недавно погибшего человека при помощи скиллчеков.")
        {
        }

        public override TimeSpan CooldownTime { get; set; } =
            TimeSpan.Zero;

        protected override void HandleUsing(Player player)
        {
            if (VeryUsualDay.Instance == null ||
                !VeryUsualDay.Instance.IsEnabledInRound)
            {
                return;
            }

            if (!IsMedic(player))
                return;

            if (Sessions.TryGetValue(
                    player.Id,
                    out ReviveSession existingSession))
            {
                if (existingSession.SkillCheckActive)
                {
                    if (existingSession.SkillCheckResolved)
                        return;

                    existingSession.SkillCheckSuccess =
                        existingSession.MarkerPosition >=
                        existingSession.SuccessZoneStart &&
                        existingSession.MarkerPosition <
                        existingSession.SuccessZoneStart +
                        existingSession.SuccessZoneWidth;

                    existingSession.SkillCheckResolved = true;
                    return;
                }

                CancelRevive(player, true);
                return;
            }

            DeathRecord record = FindReviveableCorpse(player);

            if (record == null)
            {
                player.Broadcast(
                    3,
                    "<color=red>Перед вами нет человека, которого можно реанимировать.</color>",
                    shouldClearPrevious: true);

                return;
            }

            if (GetDeathAge(record) > ReviveWindow)
            {
                player.Broadcast(
                    3,
                    "<color=red>Этого человека уже слишком поздно реанимировать.</color>",
                    shouldClearPrevious: true);

                return;
            }

            if (record.ActiveMedicId.HasValue)
            {
                player.Broadcast(
                    3,
                    "<color=red>Этого человека уже реанимирует другой медик.</color>",
                    shouldClearPrevious: true);

                return;
            }

            ReviveSession session = new ReviveSession
            {
                Medic = player,
                Record = record,
                Progress = 0,
            };

            record.ActiveMedicId = player.Id;
            Sessions[player.Id] = session;

            player.Broadcast(
                2,
                "<color=#55ffff><b>Реанимация начата.</b></color>\n" +
                "Оставайтесь рядом с пострадавшим и смотрите на него.",
                shouldClearPrevious: true);

            Timing.RunCoroutine(ReviveCoroutine(session));
        }

        public static void OnDying(Player player)
        {
            if (player == null)
                return;

            CancelRevive(player, false);

            WaitingForNewLife.Add(player.Id);

            PendingDeaths.Remove(player.Id);

            if (IsScpObject(player))
                return;

            if (RevivedThisLife.Contains(player.Id))
                return;

            DeathSnapshot snapshot = new DeathSnapshot
            {
                PlayerId = player.Id,
                UserId = player.UserId,

                DeathTime = DateTime.UtcNow,

                Role = player.Role.Type,

                MaxHealth = player.MaxHealth,

                CustomInfo = player.CustomInfo,
                CustomName = player.CustomName,

                Scale = player.Scale,

                WasEngineer = IsSessionFlagEnabled(
                    player,
                    "isEngineer"),

                WasMedic = IsSessionFlagEnabled(
                    player,
                    "isMedic"),

                AutoCleanup =
                    VeryUsualDay.Instance != null &&
                    (VeryUsualDay.Instance.CurrentCode ==
                     VeryUsualDay.Codes.Green ||
                     VeryUsualDay.Instance.CurrentCode ==
                     VeryUsualDay.Codes.Emerald),

                Effects = CaptureEffects(player),
            };

            PendingDeaths[player.Id] = snapshot;
        }
        public static bool OnDied(Player player)
        {
            if (player == null)
                return false;

            if (!PendingDeaths.TryGetValue(
                    player.Id,
                    out DeathSnapshot snapshot))
            {
                return false;
            }

            PendingDeaths.Remove(player.Id);

            Timing.RunCoroutine(
                RegisterCorpseCoroutine(
                    player,
                    snapshot));

            return true;
        }

        private static IEnumerator<float> RegisterCorpseCoroutine(
            Player player,
            DeathSnapshot snapshot)
        {
            Ragdoll ragdoll = null;

            for (int i = 0; i < 10; i++)
            {
                ragdoll = Ragdoll.GetLast(player);

                if (IsRagdollAvailable(ragdoll))
                    break;

                yield return Timing.WaitForSeconds(0.05f);
            }

            if (!IsRagdollAvailable(ragdoll))
            {
                Log.Warn(
                    $"[MedicRevive] Не удалось найти ragdoll игрока " +
                    $"{snapshot.UserId}.");

                yield break;
            }

            if (!TryGetSamePlayer(
                    snapshot.PlayerId,
                    snapshot.UserId,
                    out _))
            {
                ScheduleRagdollDestroy(
                    ragdoll,
                    DisconnectedCleanupDelay);

                yield break;
            }

            if (!WaitingForNewLife.Contains(snapshot.PlayerId))
            {
                if (snapshot.AutoCleanup)
                {
                    ScheduleRagdollDestroy(
                        ragdoll,
                        DisconnectedCleanupDelay);
                }

                yield break;
            }

            DeathRecord record = new DeathRecord
            {
                Snapshot = snapshot,
                Ragdoll = ragdoll,
            };

            DeathRecords[snapshot.PlayerId] = record;

            Timing.RunCoroutine(
                CorpseCleanupCoroutine(record));
        }

        private static IEnumerator<float> CorpseCleanupCoroutine(
            DeathRecord record)
        {
            while (!record.Invalidated)
            {
                if (!DeathRecords.TryGetValue(
                        record.Snapshot.PlayerId,
                        out DeathRecord currentRecord) ||
                    currentRecord != record)
                {
                    yield break;
                }

                if (!IsRagdollAvailable(record.Ragdoll))
                {
                    RemoveRecordWithoutDestroy(record);
                    yield break;
                }

                if (!TryGetSamePlayer(
                        record.Snapshot.PlayerId,
                        record.Snapshot.UserId,
                        out _))
                {
                    RemoveRecordWithoutDestroy(record);

                    ScheduleRagdollDestroy(
                        record.Ragdoll,
                        DisconnectedCleanupDelay);

                    yield break;
                }

                float age = GetDeathAge(record);

                if (age < ReviveWindow)
                {
                    yield return Timing.WaitForSeconds(0.25f);
                    continue;
                }

                if (record.ActiveMedicId.HasValue)
                {
                    if (Sessions.ContainsKey(
                            record.ActiveMedicId.Value))
                    {
                        yield return Timing.WaitForSeconds(0.25f);
                        continue;
                    }

                    record.ActiveMedicId = null;
                }

                if (!record.Snapshot.AutoCleanup)
                {
                    RemoveRecordWithoutDestroy(record);
                    yield break;
                }

                if (!record.GraceCleanupTime.HasValue)
                {
                    DestroyRecord(record);
                    yield break;
                }

                if (DateTime.UtcNow >=
                    record.GraceCleanupTime.Value)
                {
                    DestroyRecord(record);
                    yield break;
                }

                yield return Timing.WaitForSeconds(0.25f);
            }
        }

        private static IEnumerator<float> ReviveCoroutine(
            ReviveSession session)
        {
            while (!session.Cancelled &&
                   session.Progress < 100)
            {
                if (!ValidateSession(session))
                    yield break;

                StartSkillCheck(session);

                for (int i = 0; i < BarLength; i++)
                {
                    if (!ValidateSession(session))
                        yield break;

                    session.MarkerPosition = i;

                    ShowSkillCheckBroadcast(session);

                    yield return Timing.WaitForSeconds(
                        MarkerStepTime);

                    if (session.SkillCheckResolved)
                        break;
                }

                session.SkillCheckActive = false;

                bool success =
                    session.SkillCheckResolved &&
                    session.SkillCheckSuccess;

                session.SkillCheckResolved = false;

                if (success)
                {
                    int addedProgress =
                        UnityEngine.Random.Range(10, 19);

                    session.Progress = Mathf.Clamp(
                        session.Progress + addedProgress,
                        0,
                        100);

                    session.LastResult =
                        $"<color=#55ff55><b>УСПЕХ</b></color> " +
                        $"+{addedProgress}%";
                }
                else
                {
                    int removedProgress =
                        UnityEngine.Random.Range(4, 9);

                    session.Progress = Mathf.Clamp(
                        session.Progress - removedProgress,
                        0,
                        100);

                    session.LastResult =
                        $"<color=red><b>ПРОВАЛ</b></color> " +
                        $"-{removedProgress}%";
                }
            }

            if (session.Cancelled)
                yield break;

            if (!Sessions.TryGetValue(
                    session.Medic.Id,
                    out ReviveSession currentSession) ||
                currentSession != session)
            {
                yield break;
            }

            CompleteRevive(session);
        }

        private static void StartSkillCheck(
            ReviveSession session)
        {
            session.SkillCheckActive = true;
            session.SkillCheckResolved = false;
            session.SkillCheckSuccess = false;
            session.MarkerPosition = 0;

            float age = GetDeathAge(session.Record);

            if (age <= 20f)
            {
                session.SuccessZoneWidth =
                    FreshSuccessZoneWidth;
            }
            else if (age <= 40f)
            {
                session.SuccessZoneWidth =
                    MediumSuccessZoneWidth;
            }
            else
            {
                session.SuccessZoneWidth =
                    OldSuccessZoneWidth;
            }

            session.SuccessZoneStart =
                UnityEngine.Random.Range(
                    1,
                    BarLength -
                    session.SuccessZoneWidth);
        }

        private static void ShowSkillCheckBroadcast(
            ReviveSession session)
        {
            string bar = BuildSkillCheckBar(session);

            string resultText =
                string.IsNullOrEmpty(session.LastResult)
                    ? string.Empty
                    : $"\n{session.LastResult}";

            session.Medic.Broadcast(
                1,
                "<color=#55ffff><b>Реанимация</b></color>\n" +
                $"Прогресс: <b>{session.Progress}%</b>\n" +
                $"{bar}" +
                resultText,
                shouldClearPrevious: true);
        }

        private static string BuildSkillCheckBar(
            ReviveSession session)
        {
            StringBuilder builder =
                new StringBuilder();

            for (int i = 0; i < BarLength; i++)
            {
                bool isSuccessZone =
                    i >= session.SuccessZoneStart &&
                    i <
                    session.SuccessZoneStart +
                    session.SuccessZoneWidth;

                if (i == session.MarkerPosition)
                {
                    builder.Append(
                        isSuccessZone
                            ? "<color=#ffffff>●</color>"
                            : "<color=#ffcc00>●</color>");
                }
                else if (isSuccessZone)
                {
                    builder.Append(
                        "<color=#55ff55>━</color>");
                }
                else
                {
                    builder.Append(
                        "<color=#666666>━</color>");
                }
            }

            return builder.ToString();
        }

        private static bool ValidateSession(
            ReviveSession session)
        {
            if (session.Cancelled)
                return false;

            Player medic = session.Medic;
            DeathRecord record = session.Record;

            if (VeryUsualDay.Instance == null ||
                !VeryUsualDay.Instance.IsEnabledInRound ||
                medic == null ||
                medic.IsDead ||
                !IsMedic(medic))
            {
                CancelSession(
                    session,
                    false);

                return false;
            }

            if (record.Invalidated ||
                !DeathRecords.TryGetValue(
                    record.Snapshot.PlayerId,
                    out DeathRecord currentRecord) ||
                currentRecord != record)
            {
                CancelSession(
                    session,
                    true,
                    "<color=red><b>Реанимация прервана.</b></color>");

                return false;
            }

            if (!TryGetSamePlayer(
                    record.Snapshot.PlayerId,
                    record.Snapshot.UserId,
                    out Player target))
            {
                InvalidateForDisconnect(record);
                return false;
            }

            if (!WaitingForNewLife.Contains(target.Id))
            {
                InvalidateForNewLife(record);
                return false;
            }

            if (IsScpObject(target))
            {
                InvalidateForNewLife(record);
                return false;
            }

            if (!IsRagdollAvailable(record.Ragdoll))
            {
                RemoveRecordWithoutDestroy(record);

                CancelSession(
                    session,
                    true,
                    "<color=red><b>Реанимация прервана: тело недоступно.</b></color>");

                return false;
            }

            if (GetDistanceToRagdoll(
                    medic,
                    record.Ragdoll) >
                MaxReviveDistance)
            {
                CancelSession(
                    session,
                    true);

                return false;
            }

            if (GetLookDot(
                    medic,
                    record.Ragdoll) <
                MinLookDot)
            {
                CancelSession(
                    session,
                    true);

                return false;
            }

            return true;
        }

        private static DeathRecord FindReviveableCorpse(
            Player medic)
        {
            DeathRecord bestRecord = null;

            float bestDot = MinLookDot;
            float bestDistance = float.MaxValue;

            foreach (DeathRecord record in
                     DeathRecords.Values.ToArray())
            {
                if (record.Invalidated)
                    continue;

                if (GetDeathAge(record) >
                    ReviveWindow)
                {
                    continue;
                }

                if (!IsRagdollAvailable(record.Ragdoll))
                    continue;

                if (!TryGetSamePlayer(
                        record.Snapshot.PlayerId,
                        record.Snapshot.UserId,
                        out Player target))
                {
                    continue;
                }

                if (!WaitingForNewLife.Contains(target.Id))
                    continue;

                if (IsScpObject(target))
                    continue;

                float distance =
                    GetDistanceToRagdoll(
                        medic,
                        record.Ragdoll);

                if (distance > MaxReviveDistance)
                    continue;

                float dot =
                    GetLookDot(
                        medic,
                        record.Ragdoll);

                if (dot < MinLookDot)
                    continue;

                if (dot > bestDot + 0.01f)
                {
                    bestRecord = record;
                    bestDot = dot;
                    bestDistance = distance;
                    continue;
                }

                if (Mathf.Abs(dot - bestDot) <= 0.01f &&
                    distance < bestDistance)
                {
                    bestRecord = record;
                    bestDistance = distance;
                }
            }

            return bestRecord;
        }

        private static void CompleteRevive(
            ReviveSession session)
        {
            if (session.Cancelled)
                return;

            DeathRecord record = session.Record;

            if (!TryGetSamePlayer(
                    record.Snapshot.PlayerId,
                    record.Snapshot.UserId,
                    out Player target))
            {
                InvalidateForDisconnect(record);
                return;
            }

            if (!WaitingForNewLife.Contains(target.Id) ||
                IsScpObject(target) ||
                !IsRagdollAvailable(record.Ragdoll))
            {
                InvalidateForNewLife(record);
                return;
            }

            Sessions.Remove(session.Medic.Id);

            record.ActiveMedicId = null;

            DeathRecords.Remove(
                record.Snapshot.PlayerId);

            record.Invalidated = true;

            Vector3 revivePosition =
            record.Ragdoll.Position + Vector3.up * 0.5f;

            Ragdoll ragdoll =
                record.Ragdoll;

            DeathSnapshot snapshot =
                record.Snapshot;

            RevivedThisLife.Add(target.Id);

            WaitingForNewLife.Remove(target.Id);

            RevivalRoleChanges.Add(target.Id);

            target.Role.Set(
                snapshot.Role,
                reason: SpawnReason.ForceClass,
                spawnFlags: RoleSpawnFlags.None);

            Timing.CallDelayed(0.15f, () =>
            {
                try
                {
                    if (!TryGetSamePlayer(
                            snapshot.PlayerId,
                            snapshot.UserId,
                            out Player revivedPlayer))
                    {
                        ScheduleRagdollDestroy(
                            ragdoll,
                            DisconnectedCleanupDelay);

                        return;
                    }

                    revivedPlayer.ClearInventory(
                        destroy: true);

                    revivedPlayer.DisableAllEffects();

                    revivedPlayer.MaxHealth =
                        snapshot.MaxHealth;

                    revivedPlayer.Health =
                        Mathf.Max(
                            1f,
                            snapshot.MaxHealth * 0.1f);

                    revivedPlayer.Scale =
                        snapshot.Scale;

                    revivedPlayer.CustomInfo =
                        snapshot.CustomInfo;

                    revivedPlayer.CustomName =
                        snapshot.CustomName;

                    revivedPlayer.SessionVariables[
                        "isEngineer"] =
                        snapshot.WasEngineer;

                    revivedPlayer.SessionVariables[
                        "isMedic"] =
                        snapshot.WasMedic;

                    revivedPlayer.SessionVariables[
                        "isCustomScp"] = false;

                    foreach (EffectSnapshot effect in
                             snapshot.Effects)
                    {
                        revivedPlayer.EnableEffect(
                            effect.Name,
                            effect.Intensity,
                            effect.Duration);
                    }

                    revivedPlayer.Teleport(
                        revivePosition);

                    if (IsRagdollAvailable(ragdoll))
                        ragdoll.Destroy();

                    session.Medic.Broadcast(
                        4,
                        "<color=#55ff55><b>Реанимация успешно завершена.</b></color>",
                        shouldClearPrevious: true);

                    revivedPlayer.Broadcast(
                        5,
                        "<color=#55ff55><b>Вы были реанимированы медиком.</b></color>",
                        shouldClearPrevious: true);
                }
                finally
                {
                    RevivalRoleChanges.Remove(
                        snapshot.PlayerId);
                }
            });
        }

        public static void OnChangingRole(
            Player player,
            RoleTypeId newRole,
            SpawnReason reason)
        {
            if (player == null)
                return;

            CancelRevive(player, false);

            if (RevivalRoleChanges.Remove(player.Id))
                return;

            if (newRole == RoleTypeId.Spectator)
                return;

            if (newRole == RoleTypeId.Tutorial &&
                WaitingForNewLife.Contains(player.Id) &&
                DeathLobbyTransitions.Remove(player.Id))
            {
                return;
            }
            MarkNewLife(player);
        }

        public static void MarkDeathLobbyTransition(
            Player player)
        {
            if (player == null)
                return;

            if (!WaitingForNewLife.Contains(player.Id))
                return;

            DeathLobbyTransitions.Add(player.Id);
        }

        public static bool IsWaitingForNewLife(
            Player player)
        {
            return player != null &&
                   WaitingForNewLife.Contains(player.Id);
        }

        public static void MarkNewLife(
            Player player)
        {
            if (player == null)
                return;

            int playerId = player.Id;

            WaitingForNewLife.Remove(playerId);
            RevivedThisLife.Remove(playerId);
            DeathLobbyTransitions.Remove(playerId);
            PendingDeaths.Remove(playerId);

            if (DeathRecords.TryGetValue(
                    playerId,
                    out DeathRecord record))
            {
                InvalidateForNewLife(record);
            }
        }


        public static void MarkCustomScpSpawn(
            Player player)
        {
            if (player == null)
                return;

            MarkNewLife(player);

            player.SessionVariables[
                "isCustomScp"] = true;
        }

        public static void OnLeft(Player player)
        {
            if (player == null)
                return;

            CancelRevive(player, false);

            PendingDeaths.Remove(player.Id);

            WaitingForNewLife.Remove(player.Id);
            RevivedThisLife.Remove(player.Id);
            DeathLobbyTransitions.Remove(player.Id);
            RevivalRoleChanges.Remove(player.Id);

            if (!DeathRecords.TryGetValue(
                    player.Id,
                    out DeathRecord record))
            {
                return;
            }

            if (record.ActiveMedicId.HasValue &&
                Sessions.TryGetValue(
                    record.ActiveMedicId.Value,
                    out ReviveSession medicSession))
            {
                medicSession.Cancelled = true;
                medicSession.SkillCheckActive = false;

                Sessions.Remove(
                    medicSession.Medic.Id);

                medicSession.Medic.Broadcast(
                    3,
                    "<color=red><b>Реанимация прервана: игрок покинул сервер.</b></color>",
                    shouldClearPrevious: true);
            }

            record.ActiveMedicId = null;
            record.Invalidated = true;

            DeathRecords.Remove(player.Id);

            ScheduleRagdollDestroy(
                record.Ragdoll,
                DisconnectedCleanupDelay);
        }

        public static void CancelRevive(
            Player player,
            bool showBroadcast)
        {
            if (player == null)
                return;

            if (!Sessions.TryGetValue(
                    player.Id,
                    out ReviveSession session))
            {
                return;
            }

            CancelSession(
                session,
                showBroadcast);
        }

        private static void CancelSession(
            ReviveSession session,
            bool showBroadcast,
            string customMessage = null)
        {
            if (session == null ||
                session.Cancelled)
            {
                return;
            }

            session.Cancelled = true;
            session.SkillCheckActive = false;

            Sessions.Remove(
                session.Medic.Id);

            DeathRecord record =
                session.Record;

            if (record != null &&
                !record.Invalidated &&
                record.ActiveMedicId ==
                session.Medic.Id)
            {
                record.ActiveMedicId = null;

                if (GetDeathAge(record) >
                    ReviveWindow)
                {
                    if (record.Snapshot.AutoCleanup)
                    {
                        record.GraceCleanupTime =
                            DateTime.UtcNow.AddSeconds(
                                ExpiredInterruptedCleanupDelay);
                    }
                    else
                    {
                        RemoveRecordWithoutDestroy(record);
                    }
                }
            }

            if (showBroadcast &&
                session.Medic != null)
            {
                session.Medic.Broadcast(
                    3,
                    customMessage ??
                    "<color=red><b>Реанимация прервана.</b></color>",
                    shouldClearPrevious: true);
            }
        }

        private static void InvalidateForNewLife(
            DeathRecord record)
        {
            if (record == null ||
                record.Invalidated)
            {
                return;
            }

            record.Invalidated = true;

            DeathRecords.Remove(
                record.Snapshot.PlayerId);

            if (record.ActiveMedicId.HasValue &&
                Sessions.TryGetValue(
                    record.ActiveMedicId.Value,
                    out ReviveSession session))
            {
                session.Cancelled = true;
                session.SkillCheckActive = false;

                Sessions.Remove(
                    session.Medic.Id);

                session.Medic.Broadcast(
                    3,
                    "<color=red><b>Реанимация прервана: персонаж уже получил новую жизнь.</b></color>",
                    shouldClearPrevious: true);
            }

            record.ActiveMedicId = null;

            if (record.Snapshot.AutoCleanup)
            {
                ScheduleRagdollDestroy(
                    record.Ragdoll,
                    DisconnectedCleanupDelay);
            }
        }

        private static void InvalidateForDisconnect(
            DeathRecord record)
        {
            if (record == null ||
                record.Invalidated)
            {
                return;
            }

            record.Invalidated = true;

            DeathRecords.Remove(
                record.Snapshot.PlayerId);

            if (record.ActiveMedicId.HasValue &&
                Sessions.TryGetValue(
                    record.ActiveMedicId.Value,
                    out ReviveSession session))
            {
                session.Cancelled = true;
                session.SkillCheckActive = false;

                Sessions.Remove(
                    session.Medic.Id);

                session.Medic.Broadcast(
                    3,
                    "<color=red><b>Реанимация прервана: игрок покинул сервер.</b></color>",
                    shouldClearPrevious: true);
            }

            record.ActiveMedicId = null;

            ScheduleRagdollDestroy(
                record.Ragdoll,
                DisconnectedCleanupDelay);
        }

        private static void DestroyRecord(
            DeathRecord record)
        {
            if (record == null)
                return;

            record.Invalidated = true;

            if (DeathRecords.TryGetValue(
                    record.Snapshot.PlayerId,
                    out DeathRecord current) &&
                current == record)
            {
                DeathRecords.Remove(
                    record.Snapshot.PlayerId);
            }

            if (IsRagdollAvailable(record.Ragdoll))
                record.Ragdoll.Destroy();
        }

        private static void RemoveRecordWithoutDestroy(
            DeathRecord record)
        {
            if (record == null)
                return;

            record.Invalidated = true;

            if (DeathRecords.TryGetValue(
                    record.Snapshot.PlayerId,
                    out DeathRecord current) &&
                current == record)
            {
                DeathRecords.Remove(
                    record.Snapshot.PlayerId);
            }
        }

        private static void ScheduleRagdollDestroy(
            Ragdoll ragdoll,
            float delay)
        {
            if (ragdoll == null)
                return;

            Timing.CallDelayed(delay, () =>
            {
                if (IsRagdollAvailable(ragdoll))
                    ragdoll.Destroy();
            });
        }

        private static List<EffectSnapshot> CaptureEffects(
            Player player)
        {
            List<EffectSnapshot> effects =
                new List<EffectSnapshot>();

            foreach (StatusEffectBase effect in
                     player.ActiveEffects.ToArray())
            {
                if (effect == null)
                    continue;

                effects.Add(
                    new EffectSnapshot
                    {
                        Name =
                            effect.GetType().Name,

                        Intensity =
                            effect.Intensity,

                        Duration =
                            effect.Duration,
                    });
            }

            return effects;
        }

        private static bool IsScpObject(
            Player player)
        {
            if (player == null)
                return false;

            if (player.Role.Type.GetSide() ==
                Side.Scp)
            {
                return true;
            }

            if (player.TryGetSessionVariable(
                    "isCustomScp",
                    out bool isCustomScp) &&
                isCustomScp)
            {
                return true;
            }

            if (VeryUsualDay.Instance != null &&
                VeryUsualDay.Instance.ScpPlayers.ContainsKey(
                    player.Id))
            {
                return true;
            }

            return false;
        }

        private static bool IsMedic(
            Player player)
        {
            return player != null &&
                   player.TryGetSessionVariable(
                       "isMedic",
                       out bool isMedic) &&
                   isMedic;
        }

        private static bool IsSessionFlagEnabled(
            Player player,
            string key)
        {
            return player.TryGetSessionVariable(
                       key,
                       out bool value) &&
                   value;
        }

        private static bool TryGetSamePlayer(
            int playerId,
            string userId,
            out Player player)
        {
            player = null;

            if (!Player.TryGet(
                    playerId,
                    out Player foundPlayer))
            {
                return false;
            }

            if (!string.Equals(
                    foundPlayer.UserId,
                    userId,
                    StringComparison.Ordinal))
            {
                return false;
            }

            player = foundPlayer;
            return true;
        }

        private static bool IsRagdollAvailable(
            Ragdoll ragdoll)
        {
            return ragdoll != null &&
                   ragdoll.IsSpawned;
        }

        private static float GetDeathAge(
            DeathRecord record)
        {
            return (float)
                (DateTime.UtcNow -
                 record.Snapshot.DeathTime)
                .TotalSeconds;
        }

        private static float GetDistanceToRagdoll(
            Player player,
            Ragdoll ragdoll)
        {
            return Vector3.Distance(
                player.Position,
                ragdoll.Position);
        }

        private static float GetLookDot(
            Player player,
            Ragdoll ragdoll)
        {
            Vector3 direction =
                ragdoll.Position -
                player.CameraTransform.position;

            if (direction.sqrMagnitude < 0.001f)
                return 1f;

            Vector3 forward =
                player.CameraTransform.forward;

            if (forward.sqrMagnitude < 0.001f)
                return -1f;

            return Vector3.Dot(
                forward.normalized,
                direction.normalized);
        }

        public static void ResetAll()
        {
            foreach (ReviveSession session in
                     Sessions.Values.ToArray())
            {
                session.Cancelled = true;
                session.SkillCheckActive = false;
            }

            Sessions.Clear();
            PendingDeaths.Clear();
            DeathRecords.Clear();

            RevivedThisLife.Clear();
            WaitingForNewLife.Clear();
            DeathLobbyTransitions.Clear();
            RevivalRoleChanges.Clear();
        }

        private sealed class EffectSnapshot
        {
            public string Name;
            public byte Intensity;
            public float Duration;
        }

        private sealed class DeathSnapshot
        {
            public int PlayerId;
            public string UserId;

            public DateTime DeathTime;

            public RoleTypeId Role;

            public float MaxHealth;

            public string CustomInfo;
            public string CustomName;

            public Vector3 Scale;

            public bool WasEngineer;
            public bool WasMedic;

            public bool AutoCleanup;

            public List<EffectSnapshot> Effects;
        }

        private sealed class DeathRecord
        {
            public DeathSnapshot Snapshot;

            public Ragdoll Ragdoll;

            public int? ActiveMedicId;

            public DateTime? GraceCleanupTime;

            public bool Invalidated;
        }

        private sealed class ReviveSession
        {
            public Player Medic;

            public DeathRecord Record;

            public int Progress;

            public int MarkerPosition;
            public int SuccessZoneStart;
            public int SuccessZoneWidth;

            public bool SkillCheckActive;
            public bool SkillCheckResolved;
            public bool SkillCheckSuccess;

            public bool Cancelled;

            public string LastResult;
        }
    }
}