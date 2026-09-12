using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;
using VeryUsualDay.Abilities.Medic;

namespace VeryUsualDay.Utils
{
    public class Scp035
    {
        private Player User { get; set; }
        
        public Scp035(Player player)
        {
            User = player;
            _spawn();
        }

        private void _spawn()
        {
            MedicReviveAbility.MarkCustomScpSpawn(User);
            User.Role.Set(RoleTypeId.Scp0492, reason: SpawnReason.ForceClass, spawnFlags: RoleSpawnFlags.AssignInventory);
            Timing.CallDelayed(2f, () =>
            {
                User.CustomInfo = "<b><color=#960018>SCP-035</color></b>";
                User.MaxHealth = 6000f;
                User.Health = 6000f;
                User.Scale = new Vector3(1f, 0.1f, 1f);
                User.EnableEffect(EffectType.DamageReduction);
                User.ChangeEffectIntensity(EffectType.DamageReduction, 40);
                User.EnableEffect(EffectType.Slowness);
                User.ChangeEffectIntensity(EffectType.Slowness, 70);
                User.EnableEffect(EffectType.SilentWalk);
                User.ChangeEffectIntensity(EffectType.SilentWalk, 10);
                User.IsGodModeEnabled = false;
                VeryUsualDay.Instance.ScpPlayers.Add(User.Id, VeryUsualDay.Scps.Scp035);
            });
        }
    }
}