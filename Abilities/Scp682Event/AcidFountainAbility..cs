using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace VeryUsualDay.Abilities.Scp682Event
{
    public class AcidFountainAbility : Scp682EventAbility
    {
        public AcidFountainAbility()
            : base(
                68201,
                "SCP-682: Кислотный фонтан",
                KeyCode.Alpha2,
                "Последовательно бросает 8 гранат с интервалом 0,5 секунды. Запал — 5 секунд, урон снижен вдвое. Кулдаун — 40 секунд.")
        {
        }

        public override TimeSpan CooldownTime { get; set; } =
            TimeSpan.FromSeconds(40f);

        protected override void HandleUsing(Player player)
        {
            if (!TryBeginUse(player))
                return;

            Timing.RunCoroutine(ThrowGrenades(player));
        }

        private static IEnumerator<float> ThrowGrenades(Player player)
        {
            for (int i = 0; i < 8; i++)
            {
                if (!Scp682EventAbilityManager.IsScp682Event(player))
                    yield break;

                Scp682EventAbilityManager.ThrowGrenade(
                    player,
                    fuseTime: 5f,
                    damageMultiplier: 0.5f,
                    applyPoison: false);

                if (i < 7)
                    yield return Timing.WaitForSeconds(0.5f);
            }
        }
    }
}