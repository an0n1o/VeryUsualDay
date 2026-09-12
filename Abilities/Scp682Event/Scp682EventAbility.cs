using Exiled.API.Features;
using UnityEngine;

namespace VeryUsualDay.Abilities.Scp682Event
{
    public abstract class Scp682EventAbility : BaseAbility
    {
        protected Scp682EventAbility(
            int id,
            string label,
            KeyCode suggestedKey,
            string description)
            : base(id, label, suggestedKey, description)
        {
        }

        protected bool TryBeginUse(Player player)
        {
            if (!Scp682EventAbilityManager.IsScp682Event(player) ||
                IsInCooldown)
            {
                return false;
            }

            IsInCooldown = true;
            return true;
        }
    }
}