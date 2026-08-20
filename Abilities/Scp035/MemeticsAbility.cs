using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace VeryUsualDay.Abilities.Scp035
{
    public class MemeticsAbility : BaseAbility
    {
        public MemeticsAbility() : base(
            1,
            "SCP-035: Меметика",
            KeyCode.M,
            "Позволяет напасть на игрока рядом. Игрок теряет возможность атаковать на 15 секунд, а также получает дебаффы. Кулдаун - 20 секунд."
        )
        {
        }

        public override TimeSpan CooldownTime { get; set; } = TimeSpan.FromSeconds(20f);

        protected override void HandleUsing(Player player)
        {

            if (!VeryUsualDay.Instance.IsEnabledInRound)
                return;

            if (!VeryUsualDay.Instance.ScpPlayers.TryGetValue(player.Id, out var value) ||
                value != VeryUsualDay.Scps.Scp035 ||
                IsInCooldown)
                return;

            if (player.TryGetSessionVariable("memeticsDeathCooldown", out DateTime cooldownUntil) &&
    DateTime.UtcNow < cooldownUntil)
                return;

            if (player.TryGetSessionVariable("haveBody", out bool result) && result)
                return;

            if (player.Health <= 2000f)
                return;

            if (!Physics.Raycast(
                    new Ray(
                        player.CameraTransform.position + player.CameraTransform.forward,
                        player.CameraTransform.forward),
                    out var raycastHit,
                    6.1f,
                    Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction.Collide))
                return;

            if (!Player.TryGet(raycastHit.collider, out var target) || target == player)
                return;

            if (target.IsScp() || target.CustomInfo == "[Ликвидатор БУО]")
                return;

            IsInCooldown = true;

            if (target.CurrentItem != null && target.CurrentItem.IsFirearm)
                target.CurrentItem = null;

            target.EnableEffect(
                EffectType.Ensnared,
                duration: 5f);

            target.EnableEffect(
                EffectType.Slowness,
                85,
                15f);

            target.Broadcast(
                15,
                "<b>Вы попали под меметическую атаку <color=red>SCP-035</color>!\nУбегите или подойдите к объекту!</b>",
                shouldClearPrevious: true);

            target.SessionVariables["isUnderMemetics"] = true;

            Timing.CallDelayed(15f, () =>
            {
                target.SessionVariables["isUnderMemetics"] = false;
            });
        }
    }
}