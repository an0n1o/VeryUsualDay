using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Exiled.API.Features;
using Exiled.API.Features.Doors;

using MEC;

using UnityEngine;

namespace VeryUsualDay.Abilities.Engineer
{
    public class EngineerRepairAbility : BaseAbility
    {
        private const float MaxRepairDistance = 1f;
        private const float MinLookDot = 0.8f;

        private const int BarLength = 21;

        private const float MarkerStepTime = 0.1f;

        private const float MinCheckDelay = 0.35f;
        private const float MaxCheckDelay = 0.75f;

        private const int NormalSuccessZoneWidth = 5;

        private const int AlertSuccessZoneWidth = 3;

        private static readonly Dictionary<int, RepairSession> Sessions =
            new Dictionary<int, RepairSession>();

        public EngineerRepairAbility() : base(
            3,
            "Инженер: Ремонт двери",
            KeyCode.H,
            "Позволяет инженеру ремонтировать разрушенные двери при помощи скиллчеков.")
        {
        }

        public override TimeSpan CooldownTime { get; set; } = TimeSpan.Zero;

        protected override void HandleUsing(Player player)
        {
            if (VeryUsualDay.Instance == null ||
                !VeryUsualDay.Instance.IsEnabledInRound)
            {
                return;
            }

            if (!IsEngineer(player))
                return;

            if (Sessions.TryGetValue(player.Id, out RepairSession session))
            {
                if (session.SkillCheckActive)
                {
                    if (session.SkillCheckResolved)
                        return;

                    session.SkillCheckSuccess =
                        session.MarkerPosition >= session.SuccessZoneStart &&
                        session.MarkerPosition <
                        session.SuccessZoneStart + session.SuccessZoneWidth;

                    session.SkillCheckResolved = true;
                    return;
                }

                CancelRepair(player, true);
                return;
            }

            BreakableDoor door = FindRepairableDoor(player);

            if (door == null)
            {
                player.Broadcast(
                    3,
                    "<color=red>Перед вами нет разрушенной двери, которую можно отремонтировать.</color>",
                    shouldClearPrevious: true);

                return;
            }

            RepairSession newSession = new RepairSession
            {
                Player = player,
                Door = door,
                Progress = 0,
            };

            Sessions[player.Id] = newSession;

            player.Broadcast(
                2,
                "<color=yellow><b>Ремонт двери начат.</b></color>\n" +
                "Оставайтесь рядом и смотрите на дверной проём.",
                shouldClearPrevious: true);

            Timing.RunCoroutine(RepairCoroutine(newSession));
        }

        private static IEnumerator<float> RepairCoroutine(RepairSession session)
        {
            while (!session.Cancelled && session.Progress < 100)
            {
                if (!ValidateSession(session))
                    yield break;

                float delay = UnityEngine.Random.Range(
                    MinCheckDelay,
                    MaxCheckDelay);

                float delayPassed = 0f;

                while (delayPassed < delay)
                {
                    if (!ValidateSession(session))
                        yield break;

                    ShowWaitingBroadcast(session);

                    yield return Timing.WaitForSeconds(0.1f);

                    delayPassed += 0.1f;
                }

                if (!ValidateSession(session))
                    yield break;

                StartSkillCheck(session);

                for (int i = 0; i < BarLength; i++)
                {
                    if (!ValidateSession(session))
                        yield break;

                    session.MarkerPosition = i;

                    ShowSkillCheckBroadcast(session);

                    yield return Timing.WaitForSeconds(MarkerStepTime);

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

                    session.Player.Broadcast(
                        1,
                        $"<color=#55ff55><b>УСПЕХ</b></color> " +
                        $"+{addedProgress}%\n" +
                        $"Ремонт двери: <b>{session.Progress}%</b>",
                        shouldClearPrevious: true);
                }
                else
                {
                    int removedProgress =
                        UnityEngine.Random.Range(4, 9);

                    session.Progress = Mathf.Clamp(
                        session.Progress - removedProgress,
                        0,
                        100);

                    session.Player.Broadcast(
                        1,
                        $"<color=red><b>ПРОВАЛ</b></color> " +
                        $"-{removedProgress}%\n" +
                        $"Ремонт двери: <b>{session.Progress}%</b>",
                        shouldClearPrevious: true);
                }

                if (session.Progress >= 100)
                    break;

                float resultDelay = 0f;

                while (resultDelay < 0.5f)
                {
                    if (!ValidateSession(session))
                        yield break;

                    yield return Timing.WaitForSeconds(0.1f);

                    resultDelay += 0.1f;
                }
            }

            if (session.Cancelled)
                yield break;

            if (!Sessions.ContainsKey(session.Player.Id))
                yield break;

            Sessions.Remove(session.Player.Id);

            if (!session.Door.IsDestroyed)
            {
                session.Player.Broadcast(
                    3,
                    "<color=#55ff55>Дверь уже была отремонтирована.</color>",
                    shouldClearPrevious: true);

                yield break;
            }

            session.Door.Repair();

            session.Player.Broadcast(
                4,
                "<color=#55ff55><b>Дверь успешно отремонтирована.</b></color>",
                shouldClearPrevious: true);
        }

        private static void StartSkillCheck(RepairSession session)
        {
            session.SkillCheckActive = true;
            session.SkillCheckResolved = false;
            session.SkillCheckSuccess = false;
            session.MarkerPosition = 0;

            session.SuccessZoneWidth = IsHighAlert()
                ? AlertSuccessZoneWidth
                : NormalSuccessZoneWidth;

            session.SuccessZoneStart = UnityEngine.Random.Range(
                1,
                BarLength - session.SuccessZoneWidth);
        }

        private static void ShowWaitingBroadcast(RepairSession session)
        {
            session.Player.Broadcast(
                1,
                "<color=yellow><b>Ремонт двери</b></color>\n" +
                $"Прогресс: <b>{session.Progress}%</b>\n" +
                "<color=#aaaaaa>Ожидание проверки...</color>",
                shouldClearPrevious: true);
        }

        private static void ShowSkillCheckBroadcast(RepairSession session)
        {
            string bar = BuildSkillCheckBar(session);

            session.Player.Broadcast(
                1,
                "<color=yellow><b>Ремонт двери</b></color>\n" +
                $"Прогресс: <b>{session.Progress}%</b>\n" +
                $"{bar}",
                shouldClearPrevious: true);
        }

        private static string BuildSkillCheckBar(RepairSession session)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < BarLength; i++)
            {
                bool isSuccessZone =
                    i >= session.SuccessZoneStart &&
                    i < session.SuccessZoneStart +
                    session.SuccessZoneWidth;

                if (i == session.MarkerPosition)
                {
                    if (isSuccessZone)
                        builder.Append("<color=#ffffff>●</color>");
                    else
                        builder.Append("<color=#ffcc00>●</color>");
                }
                else if (isSuccessZone)
                {
                    builder.Append("<color=#55ff55>━</color>");
                }
                else
                {
                    builder.Append("<color=#666666>━</color>");
                }
            }

            return builder.ToString();
        }

        private static bool ValidateSession(RepairSession session)
        {
            if (session.Cancelled)
                return false;

            Player player = session.Player;

            if (VeryUsualDay.Instance == null ||
                !VeryUsualDay.Instance.IsEnabledInRound ||
                player == null ||
                player.IsDead ||
                !IsEngineer(player))
            {
                CancelFromCoroutine(session);
                return false;
            }

            if (!session.Door.IsDestroyed)
            {
                Sessions.Remove(player.Id);

                player.Broadcast(
                    3,
                    "<color=#55ff55>Дверь уже была отремонтирована.</color>",
                    shouldClearPrevious: true);

                session.Cancelled = true;
                return false;
            }

            if (GetDistanceToDoor(player, session.Door) >
                MaxRepairDistance)
            {
                CancelFromCoroutine(session);
                return false;
            }

            if (GetLookDot(player, session.Door) <
                MinLookDot)
            {
                CancelFromCoroutine(session);
                return false;
            }

            return true;
        }

        private static BreakableDoor FindRepairableDoor(Player player)
        {
            BreakableDoor bestDoor = null;

            float bestDot = MinLookDot;
            float bestDistance = float.MaxValue;

            foreach (BreakableDoor door in
                     Door.List.OfType<BreakableDoor>())
            {
                if (!door.IsDestroyed)
                    continue;

                float distance =
                    GetDistanceToDoor(player, door);

                if (distance > MaxRepairDistance)
                    continue;

                float dot = GetLookDot(player, door);

                if (dot < MinLookDot)
                    continue;

                if (dot > bestDot + 0.01f)
                {
                    bestDoor = door;
                    bestDot = dot;
                    bestDistance = distance;
                    continue;
                }

                if (Mathf.Abs(dot - bestDot) <= 0.01f &&
                    distance < bestDistance)
                {
                    bestDoor = door;
                    bestDistance = distance;
                }
            }

            return bestDoor;
        }

        private static float GetDistanceToDoor(
            Player player,
            BreakableDoor door)
        {
            Vector3 difference =
                door.Position - player.Position;


            difference.y = 0f;

            return difference.magnitude;
        }

        private static float GetLookDot(
            Player player,
            BreakableDoor door)
        {
            Vector3 direction =
                door.Position - player.CameraTransform.position;

            direction.y = 0f;

            Vector3 forward =
                player.CameraTransform.forward;

            forward.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return 1f;

            if (forward.sqrMagnitude < 0.001f)
                return -1f;

            return Vector3.Dot(
                forward.normalized,
                direction.normalized);
        }

        private static bool IsEngineer(Player player)
        {
            return player.TryGetSessionVariable(
                       "isEngineer",
                       out bool isEngineer) &&
                   isEngineer;
        }

        private static bool IsHighAlert()
        {
            if (VeryUsualDay.Instance == null)
                return false;

            switch (VeryUsualDay.Instance.CurrentCode)
            {
                case VeryUsualDay.Codes.Orange:
                case VeryUsualDay.Codes.Yellow:
                case VeryUsualDay.Codes.Red:
                case VeryUsualDay.Codes.Clean:
                    return true;

                default:
                    return false;
            }
        }

        private static void CancelFromCoroutine(
            RepairSession session)
        {
            if (session.Cancelled)
                return;

            session.Cancelled = true;
            session.SkillCheckActive = false;

            Sessions.Remove(session.Player.Id);

            session.Player.Broadcast(
                3,
                "<color=red><b>Ремонт двери прерван.</b></color>",
                shouldClearPrevious: true);
        }

        public static void CancelRepair(
            Player player,
            bool showBroadcast)
        {
            if (player == null)
                return;

            if (!Sessions.TryGetValue(
                    player.Id,
                    out RepairSession session))
            {
                return;
            }

            session.Cancelled = true;
            session.SkillCheckActive = false;

            Sessions.Remove(player.Id);

            if (showBroadcast)
            {
                player.Broadcast(
                    3,
                    "<color=red><b>Ремонт двери прерван.</b></color>",
                    shouldClearPrevious: true);
            }
        }

        private sealed class RepairSession
        {
            public Player Player;
            public BreakableDoor Door;

            public int Progress;

            public int MarkerPosition;
            public int SuccessZoneStart;
            public int SuccessZoneWidth;

            public bool SkillCheckActive;
            public bool SkillCheckResolved;
            public bool SkillCheckSuccess;

            public bool Cancelled;
        }
    }
}