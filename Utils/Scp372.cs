using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace VeryUsualDay.Utils
{
    public class Scp372
    {
        private Player User { get; set; }

        public Scp372(Player player)
        {
            User = player;
            _spawn();
        }

        private void _spawn()
        {
            User.Role.Set(RoleTypeId.Scp939, reason: SpawnReason.ForceClass, spawnFlags: RoleSpawnFlags.AssignInventory);
            Timing.CallDelayed(2f, () =>
            {
                User.CustomInfo = "<b><color=#960018>SCP-372</color></b>";
                User.MaxHealth = 2500f;
                User.Health = 2500f;
                User.Scale = new Vector3(0.1f, 0.8f, 1f);
                User.EnableEffect(EffectType.MovementBoost);
                User.ChangeEffectIntensity(EffectType.MovementBoost, 255);
                VeryUsualDay.Instance.ScpPlayers.Add(User.Id, VeryUsualDay.Scps.Scp372);
            });
        }
    }
}