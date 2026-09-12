using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;
using VeryUsualDay.Abilities.Medic;

namespace VeryUsualDay.Utils
{
    public class Scp035Old
    {
        private Player User { get; set; }
        
        public Scp035Old(Player player)
        {
            User = player;
            _spawn();
        }

        private void _spawn()
        {
            MedicReviveAbility.MarkCustomScpSpawn(User);
            User.Role.Set(RoleTypeId.ClassD, reason: SpawnReason.ForceClass, spawnFlags: RoleSpawnFlags.AssignInventory);
            Timing.CallDelayed(2f, () =>
            {
                User.CustomInfo = "<b><color=#960018>SCP-035</color></b>";
                User.AddItem(ItemType.GunRevolver);
                User.AddAmmo(AmmoType.Ammo44Cal, 32);
                User.MaxHealth = 7500f;
                User.Health = 7500f;
                User.Scale = new Vector3(0.87f, 0.87f, 1f);
                User.EnableEffect(EffectType.BodyshotReduction);
                User.ChangeEffectIntensity(EffectType.BodyshotReduction, 15);
                User.EnableEffect(EffectType.DamageReduction);
                User.ChangeEffectIntensity(EffectType.DamageReduction, 15);
                User.EnableEffect(EffectType.Poisoned);
                User.IsGodModeEnabled = true;
                VeryUsualDay.Instance.ScpPlayers.Add(User.Id, VeryUsualDay.Scps.Scp035Old);
            });
        }
    }
}