using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;
using VeryUsualDay.Abilities.Medic;

namespace VeryUsualDay.Utils
{
    public class Scp682
    {
        private Player User { get; set; }

        public Scp682(Player player)
        {
            User = player;
            _spawn();
        }

        private void _spawn()
        {
            MedicReviveAbility.MarkCustomScpSpawn(User);
            User.Role.Set(RoleTypeId.Scp939, reason: SpawnReason.ForceClass, spawnFlags: RoleSpawnFlags.AssignInventory);
            Timing.CallDelayed(2f, () =>
            {
                User.CustomInfo = "<b><color=#960018>SCP-682-MT</color></b>";
                User.MaxHealth = 15000f;
                User.Health = 15000f;
                User.HumeShield = 5000f;
                User.Scale = new Vector3(1.2f, 1.25f, 1.2f);
                User.IsGodModeEnabled = false;
                User.EnableEffect(EffectType.Invigorated);
                User.EnableEffect(EffectType.RainbowTaste);
                User.EnableEffect(EffectType.Disabled);
                User.EnableEffect(EffectType.DamageReduction);
                User.EnableEffect(EffectType.BodyshotReduction);
                User.ChangeEffectIntensity(EffectType.DamageReduction, 30);
                User.ChangeEffectIntensity(EffectType.BodyshotReduction, 30);
                VeryUsualDay.Instance.ScpPlayers.Add(User.Id, VeryUsualDay.Scps.Scp682);
            });

        }
    }
}