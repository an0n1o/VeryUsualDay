using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;
using VeryUsualDay.Abilities.Medic;

namespace VeryUsualDay.Utils
{
    public class Scp682Event
    {
        private Player User { get; set; }

        public Scp682Event(Player player)
        {
            User = player;
            _spawn();
        }

        private void _spawn()
        {
            MedicReviveAbility.MarkCustomScpSpawn(User);
            User.Role.Set(
                RoleTypeId.Scp939,
                reason: SpawnReason.ForceClass,
                spawnFlags: RoleSpawnFlags.AssignInventory);

            Timing.CallDelayed(2f, () =>
            {
                User.CustomName = "Объект-682";
                User.CustomInfo = "<b><color=#960018>SCP-682</color></b>";

                User.MaxHealth = 80000f;
                User.Health = 80000f;

                User.MaxHumeShield = 5000f;
                User.HumeShield = 5000f;

                User.Scale = new Vector3(4f, 1.5f, 1.5f);
                User.IsGodModeEnabled = false;

                User.EnableEffect(EffectType.DamageReduction);
                User.ChangeEffectIntensity(
                    EffectType.DamageReduction,
                    80);

                User.EnableEffect(EffectType.Slowness);
                User.ChangeEffectIntensity(
                    EffectType.Slowness,
                    20);

                User.EnableEffect(EffectType.Scp1344);

                User.SessionVariables["scp682Adapted"] = false;

                VeryUsualDay.Instance.ScpPlayers.Add(
                    User.Id,
                    VeryUsualDay.Scps.Scp682Event);
            });
        }
    }
}