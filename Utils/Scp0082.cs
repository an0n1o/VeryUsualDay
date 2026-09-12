using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;
using VeryUsualDay.Abilities.Medic;

namespace VeryUsualDay.Utils
{
    public class Scp0082
    {
        private Player User { get; set; }
        
        public Scp0082(Player player, bool isPatogenZombie)
        {
            User = player;
            _spawn(isPatogenZombie);
        }

        private void _spawn(bool isPatogenZombie)
        {
            MedicReviveAbility.MarkCustomScpSpawn(User);
            User.Role.Set(RoleTypeId.Scp0492, reason: SpawnReason.ForceClass, spawnFlags: RoleSpawnFlags.AssignInventory);
            Timing.CallDelayed(2f, () =>
            {
                User.CustomInfo = "<b><color=#960018>SCP-008-2</color></b>";
                User.MaxHealth = 1500f;
                User.Health = 1500f;
                User.Scale = new Vector3(1f, 1f, 1f);
                User.EnableEffect(EffectType.Stained);
                User.EnableEffect(EffectType.Poisoned);
                User.IsGodModeEnabled = false;
                User.Broadcast(10,
                    "<b>Вы стали <color=#DC143C>SCP-008-2</color>!\n<color=#960018>Атакуйте людей</color> до конца жизни!</b>");
                User.Mute();
                if (!isPatogenZombie)
                {
                    VeryUsualDay.Instance.ScpPlayers.Add(User.Id, VeryUsualDay.Scps.Scp0082);
                }
                else
                {
                    VeryUsualDay.Instance.Zombies.Add(User.Id);
                }
            });
        }
    }
}