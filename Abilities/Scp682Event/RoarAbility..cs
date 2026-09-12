using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;

namespace VeryUsualDay.Abilities.Scp682Event
{
    public class RoarAbility : Scp682EventAbility
    {
        public RoarAbility()
            : base(
                68205,
                "SCP-682: Рёв",
                KeyCode.R,
                "Проигрывает рёв на весь сервер и накладывает на людей CardiacArrest на 3 секунды, Blurred на 6 секунд и Concussed на 12 секунд. Кулдаун — 25 секунд.")
        {
        }

        public override TimeSpan CooldownTime { get; set; } =
            TimeSpan.FromSeconds(25f);

        protected override void HandleUsing(Player player)
        {
            if (!TryBeginUse(player))
                return;

            Scp682EventAbilityManager.PlayRoar();

            foreach (Player target in Player.List)
            {
                if (!target.IsHuman)
                    continue;

                target.EnableEffect(
                    EffectType.CardiacArrest,
                    duration: 3f);

                target.EnableEffect(
                    EffectType.Blurred,
                    duration: 6f);

                target.EnableEffect(
                    EffectType.Concussed,
                    duration: 12f);
            }
        }
    }
}