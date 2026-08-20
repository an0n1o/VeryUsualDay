using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace VeryUsualDay.Utils
{
    public class Scp966
    {
        private Player User { get; set; }

        public Scp966(Player player)
        {
            User = player;
            _spawn();
        }

        private void _spawn()
        {
            User.Role.Set(RoleTypeId.Scp0492, reason: SpawnReason.ForceClass, spawnFlags: RoleSpawnFlags.AssignInventory);
            Timing.CallDelayed(2f, () =>
            {
                User.CustomInfo = "<b><color=#960018>SCP-966</color></b>";
                User.MaxHealth = 2000f;
                User.Health = 2000f;
                User.Scale = new Vector3(1f, 1f, 1f);
                User.IsGodModeEnabled = false;
                User.EnableEffect(EffectType.Fade);
                User.ChangeEffectIntensity(EffectType.Fade, 255);
                VeryUsualDay.Instance.ScpPlayers.Add(User.Id, VeryUsualDay.Scps.Scp966);
            });

        } 
    }
}