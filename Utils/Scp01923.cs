using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace VeryUsualDay.Utils
{
    public class Scp01923
    {
        private Player User { get; set; }

        public Scp01923(Player player)
        {
            User = player;
            _spawn();
        }

        private void _spawn()
        {
            User.Role.Set(RoleTypeId.Scp3114, reason: SpawnReason.ForceClass, spawnFlags: RoleSpawnFlags.AssignInventory);
            Timing.CallDelayed(2f, () =>
            {
                var hp = Random.Range(400, 900);
                User.CustomInfo = "<b><color=#960018>SCP-019-2</color></b>";
                User.CustomName = "Объект";
                User.MaxHealth = hp;
                User.Health = hp;
                User.Scale = new Vector3(0.6f, 0.6f, 0.6f);
                User.IsGodModeEnabled = false;
                User.EnableEffect(EffectType.Disabled);
                User.Teleport(VeryUsualDay.Instance.VaseCoords);
                VeryUsualDay.Instance.ScpPlayers.Add(User.Id, VeryUsualDay.Scps.Scp01923);
            });

        }
    }
}
