using System;
using Exiled.API.Features;
using UnityEngine;

namespace VeryUsualDay.Abilities.Scp682Event
{
    public class ChargeAbility : Scp682EventAbility
    {
        public ChargeAbility()
            : base(
                68203,
                "SCP-682: Разгон",
                KeyCode.C,
                "Выдаёт SCP-682 Jailbird. Кулдаун — 20 секунд.")
        {
        }

        public override TimeSpan CooldownTime { get; set; } =
            TimeSpan.FromSeconds(20f);

        protected override void HandleUsing(Player player)
        {
            if (!TryBeginUse(player))
                return;

            player.CurrentItem = player.AddItem(ItemType.Jailbird);
        }
    }
}