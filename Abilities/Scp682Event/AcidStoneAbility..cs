using System;
using Exiled.API.Features;
using UnityEngine;

namespace VeryUsualDay.Abilities.Scp682Event
{
    public class AcidStoneAbility : Scp682EventAbility
    {
        public AcidStoneAbility()
            : base(
                68202,
                "SCP-682: Кислотный камень",
                KeyCode.Alpha1,
                "Бросает гранату с запалом 2 секунды. Поражённые люди получают отравление на 10 секунд. Кулдаун — 15 секунд.")
        {
        }

        public override TimeSpan CooldownTime { get; set; } =
            TimeSpan.FromSeconds(15f);

        protected override void HandleUsing(Player player)
        {
            if (!TryBeginUse(player))
                return;

            Scp682EventAbilityManager.ThrowGrenade(
                player,
                fuseTime: 2f,
                damageMultiplier: 1f,
                applyPoison: true);
        }
    }
}