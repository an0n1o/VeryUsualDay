using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace VeryUsualDay.Abilities.Scp682Event
{
    public static class Scp682EventAbilityManager
    {
        private const string RoarClipName =
            "vud_scp682_roar";

        private const string RoarPlayerName =
            "VUD SCP-682 Roar";

        private const string RoarResourceName =
            "VeryUsualDay.Resources.682roar.ogg";

        private const string DamageContextKey =
            "scp682EventGrenadeDamage";

        private static readonly Dictionary<ushort, GrenadeData> Grenades =
            new Dictionary<ushort, GrenadeData>();
        private static bool eventsRegistered;
        public static bool IsScp682Event(Player player)
        {
            if (player == null ||
                player.IsDead ||
                VeryUsualDay.Instance == null ||
                !VeryUsualDay.Instance.IsEnabledInRound)
            {
                return false;
            }

            return VeryUsualDay.Instance.ScpPlayers.TryGetValue(
                       player.Id,
                       out VeryUsualDay.Scps scpType) &&
                   scpType == VeryUsualDay.Scps.Scp682Event;
        }

        public static void RegisterEvents()
        {
            if (eventsRegistered)
                return;

            Exiled.Events.Handlers.Map.ExplodingGrenade +=
                OnExplodingGrenade;

            Exiled.Events.Handlers.Player.Hurting +=
                OnHurting;

            eventsRegistered = true;
        }

        public static void UnregisterEvents()
        {
            if (!eventsRegistered)
                return;

            Exiled.Events.Handlers.Map.ExplodingGrenade -=
                OnExplodingGrenade;

            Exiled.Events.Handlers.Player.Hurting -=
                OnHurting;

            eventsRegistered = false;

            Reset();
        }

        public static void Reset()
        {
            Grenades.Clear();

            if (AudioPlayer.TryGet(
                    RoarPlayerName,
                    out AudioPlayer audioPlayer))
            {
                audioPlayer.RemoveAllClips();
            }
        }

        public static bool LoadRoarClip()
        {
            if (AudioClipStorage.AudioClips.ContainsKey(RoarClipName))
                return true;

            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream =
                   assembly.GetManifestResourceStream(RoarResourceName))
            {
                if (stream == null)
                {
                    Log.Error(
                        $"Не удалось найти встроенный ресурс {RoarResourceName}.");

                    return false;
                }

                using (MemoryStream buffer = new MemoryStream())
                {
                    stream.CopyTo(buffer);

                    string audioDirectory = System.IO.Path.Combine(
                        Exiled.API.Features.Paths.Configs,
                        "VeryUsualDay",
                        "Audio");

                    System.IO.Directory.CreateDirectory(audioDirectory);

                    string audioPath = System.IO.Path.Combine(
                        audioDirectory,
                        RoarClipName + ".ogg");

                    System.IO.File.WriteAllBytes(
                        audioPath,
                        buffer.ToArray());

                    return AudioClipStorage.LoadClip(
                        audioPath,
                        RoarClipName);
                }
            }
        }

        public static void UnloadRoarClip()
        {
            if (AudioPlayer.TryGet(
                    RoarPlayerName,
                    out AudioPlayer audioPlayer))
            {
                audioPlayer.Destroy();
            }

            if (AudioClipStorage.AudioClips.ContainsKey(RoarClipName))
                AudioClipStorage.DestroyClip(RoarClipName);
        }

        public static void PlayRoar()
        {
            if (!LoadRoarClip())
                return;

            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet(
                RoarPlayerName,
                onIntialCreation: player =>
                {
                    player.AddSpeaker(
                        "Main",
                        volume: 2f,
                        isSpatial: false,
                        maxDistance: 5000f);
                });

            audioPlayer?.AddClip(RoarClipName);
        }

        public static void ThrowGrenade(
            Player player,
            float fuseTime,
            float damageMultiplier,
            bool applyPoison)
        {
            ExplosiveGrenade grenade =
                Item.Create<ExplosiveGrenade>(
                    ItemType.GrenadeHE,
                    player);

            grenade.FuseTime = fuseTime;

            Grenades[grenade.Projectile.Serial] =
                new GrenadeData(
                    player.Id,
                    damageMultiplier,
                    applyPoison,
                    excludeOwner: false);

            try
            {
                player.ThrowItem(
                    grenade,
                    fullForce: true);
            }
            catch (Exception exception)
            {
                Grenades.Remove(grenade.Projectile.Serial);

                Log.Error(
                    $"Не удалось бросить гранату способности SCP-682: {exception}");
            }
            finally
            {
                grenade.Destroy();
            }
        }

        public static void SpawnHeavyJumpExplosion(Player player)
        {
            const int grenadeCount = 8;
            const float distanceFromPlayer = 2f;

            HashSet<int> affectedPlayers = new HashSet<int>();

            for (int i = 0; i < grenadeCount; i++)
            {
                float angle =
                    Mathf.PI * 2f * i / grenadeCount;

                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * distanceFromPlayer,
                    0.2f,
                    Mathf.Sin(angle) * distanceFromPlayer);

                ExplosionGrenadeProjectile grenade =
                    Projectile.Create<ExplosionGrenadeProjectile>(
                        ProjectileType.FragGrenade);

                grenade.FuseTime = 0f;
                grenade.MaxRadius = 3f;

                grenade.Spawn(
                    player.Position + offset,
                    Quaternion.identity,
                    shouldBeActive: false,
                    previousOwner: player);

                Grenades[grenade.Serial] =
                    new GrenadeData(
                        player.Id,
                        damageMultiplier: 2f,
                        applyPoison: false,
                        excludeOwner: true,
                        affectedPlayers: affectedPlayers);

                grenade.Explode();
            }
        }

        private static void OnExplodingGrenade(
            ExplodingGrenadeEventArgs ev)
        {
            if (!Grenades.TryGetValue(
                    ev.Projectile.Serial,
                    out GrenadeData data))
            {
                return;
            }

            Grenades.Remove(ev.Projectile.Serial);

            if (!Player.TryGet(data.OwnerId, out Player owner) ||
                !IsScp682Event(owner))
            {
                return;
            }

            if (data.ExcludeOwner)
                ev.TargetsToAffect.Remove(owner);

            /*
             * Все восемь гранат тяжёлого прыжка используют один
             * набор игроков. Благодаря этому один человек получает
             * двойной урон только от одной гранаты, а не от всех восьми.
             */
            if (data.AffectedPlayers != null)
            {
                foreach (Player target in
                         new List<Player>(ev.TargetsToAffect))
                {
                    if (!data.AffectedPlayers.Add(target.Id))
                        ev.TargetsToAffect.Remove(target);
                }
            }

            foreach (Player target in ev.TargetsToAffect)
            {
                DamageContext context =
                    new DamageContext(
                        data.OwnerId,
                        data.DamageMultiplier,
                        data.ApplyPoison);

                target.SessionVariables[DamageContextKey] = context;

                Timing.CallDelayed(0f, () =>
                {
                    if (target.TryGetSessionVariable(
                            DamageContextKey,
                            out DamageContext currentContext) &&
                        ReferenceEquals(currentContext, context))
                    {
                        target.SessionVariables.Remove(
                            DamageContextKey);
                    }
                });
            }
        }

        private static void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null ||
                ev.DamageHandler.Type != DamageType.Explosion ||
                !ev.Player.TryGetSessionVariable(
                    DamageContextKey,
                    out DamageContext context) ||
                context.OwnerId != ev.Attacker.Id)
            {
                return;
            }

            ev.Amount *= context.DamageMultiplier;

            if (context.ApplyPoison && ev.Player.IsHuman)
            {
                ev.Player.EnableEffect(
                    EffectType.Poisoned,
                    duration: 10f);
            }
        }

        private sealed class GrenadeData
        {
            public GrenadeData(
                int ownerId,
                float damageMultiplier,
                bool applyPoison,
                bool excludeOwner,
                HashSet<int> affectedPlayers = null)
            {
                OwnerId = ownerId;
                DamageMultiplier = damageMultiplier;
                ApplyPoison = applyPoison;
                ExcludeOwner = excludeOwner;
                AffectedPlayers = affectedPlayers;
            }

            public int OwnerId { get; }

            public float DamageMultiplier { get; }

            public bool ApplyPoison { get; }

            public bool ExcludeOwner { get; }

            public HashSet<int> AffectedPlayers { get; }
        }

        private sealed class DamageContext
        {
            public DamageContext(
                int ownerId,
                float damageMultiplier,
                bool applyPoison)
            {
                OwnerId = ownerId;
                DamageMultiplier = damageMultiplier;
                ApplyPoison = applyPoison;
            }

            public int OwnerId { get; }

            public float DamageMultiplier { get; }

            public bool ApplyPoison { get; }
        }
    }
}