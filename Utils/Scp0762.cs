using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;
using VeryUsualDay.Abilities.Medic;

namespace VeryUsualDay.Utils
{
    public class Scp0762
    {
        private Player User { get; set; }
        
        public Scp0762(Player player)
        {
            User = player;
            _spawn();
        }

        private void _spawn()
        {
            MedicReviveAbility.MarkCustomScpSpawn(User);
            User.Role.Set(RoleTypeId.Tutorial, reason: SpawnReason.ForceClass,
                spawnFlags: RoleSpawnFlags.AssignInventory);
            Timing.CallDelayed(2f, () =>
            {
                User.CustomInfo = "<b><color=#960018>SCP-076-2</color></b>";
                User.MaxHealth = 9500f;
                User.Health = 9500f;
                User.EnableEffect(EffectType.MovementBoost);
                User.EnableEffect(EffectType.BodyshotReduction);
                User.EnableEffect(EffectType.DamageReduction);
                User.ChangeEffectIntensity(EffectType.MovementBoost, 25);
                User.ChangeEffectIntensity(EffectType.BodyshotReduction, 70);
                User.ChangeEffectIntensity(EffectType.DamageReduction, 70);
                User.CurrentItem = User.AddItem(ItemType.Jailbird);
                User.Scale = new Vector3(1.15f, 1.15f, 1.15f);
                VeryUsualDay.Instance.ScpPlayers.Add(User.Id, VeryUsualDay.Scps.Scp0762);
            });

        }
    }
}