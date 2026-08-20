using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace VeryUsualDay.Abilities.Scp035
{
    public class BodyTakeoverAbility : BaseAbility
    {
        public BodyTakeoverAbility() : base(
            2,
            "SCP-035: Захват тела",
            KeyCode.Semicolon,
            "Позволяет захватить тело игрока под действием \"Меметики\", если он находится очень близко."
        ) {}
        
        public override TimeSpan CooldownTime { get; set; } = TimeSpan.Zero;
        
        protected override void HandleUsing(Player player)
        {
            if (!VeryUsualDay.Instance.IsEnabledInRound) return;
            if (!VeryUsualDay.Instance.ScpPlayers.TryGetValue(player.Id, out var value) ||
                value != VeryUsualDay.Scps.Scp035 || IsInCooldown) return;
            if (player.TryGetSessionVariable("haveBody", out bool haveBody) && haveBody) return;
            if (player.Health <= 2000f) return;
            
            if (!Physics.Raycast(
                    new Ray(player.CameraTransform.position + player.CameraTransform.forward, player.CameraTransform.forward),
                    out var raycastHit, maxDistance: 1.6f, layerMask: ~(1 << 8)
                )
               ) return;
            
            var referenceHub = raycastHit.transform?.GetComponentInParent<ReferenceHub>();
            if (referenceHub == null || !Player.TryGet(referenceHub, out var target) || target == player) return;
            if (!target.TryGetSessionVariable("isUnderMemetics", out bool isUnderMemetics) || !isUnderMemetics) return;
            
            player.SessionVariables["haveBody"] = true;
            player.SessionVariables["previousHp"] = player.Health;
            player.Role.Set(target.Role.Type, RoleSpawnFlags.None);
            player.Scale = target.Scale;
            player.CustomName = target.CustomName;
            player.Position = target.Position;
            player.MaxHealth = 220f;
            player.Health = 220f;

            foreach (var item in target.Items.ToList())
            {
                player.AddItem(item.Type);
            }

            player.AddAmmo(AmmoType.Nato9, target.GetAmmo(AmmoType.Nato9));
            player.AddAmmo(AmmoType.Nato556, target.GetAmmo(AmmoType.Nato556));
            player.AddAmmo(AmmoType.Nato762, target.GetAmmo(AmmoType.Nato762));
            player.AddAmmo(AmmoType.Ammo12Gauge, target.GetAmmo(AmmoType.Ammo12Gauge));
            player.AddAmmo(AmmoType.Ammo44Cal, target.GetAmmo(AmmoType.Ammo44Cal));

            target.ClearInventory(destroy: true);
            player.EnableEffect(EffectType.Bleeding, 1);
            player.EnableEffect(EffectType.DamageReduction, 10);
            player.IsGodModeEnabled = true;
            Timing.CallDelayed(3.5f, () => player.IsGodModeEnabled = false);
            target.Kill("Захвачен SCP-035.");
            Ragdoll.GetLast(target).Destroy();
        }
    }
}