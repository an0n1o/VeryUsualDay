using System;
using Exiled.API.Features;
using UnityEngine;

namespace VeryUsualDay.Abilities.Scp682Event
{
    public class HeavyJumpAbility : Scp682EventAbility
    {
        public HeavyJumpAbility()
            : base(
                68204,
                "SCP-682: Тяжёлый прыжок",
                KeyCode.Space,
                "При прыжке создаёт вокруг SCP-682 мгновенно взрывающиеся осколочные гранаты. Урон увеличен вдвое. Без кулдауна.")
        {
        }

        public override TimeSpan CooldownTime { get; set; } =
            TimeSpan.Zero;

        protected override void HandleUsing(Player player)
        {
            if (!TryBeginUse(player))
                return;

            Scp682EventAbilityManager.SpawnHeavyJumpExplosion(player);
        }
    }
}