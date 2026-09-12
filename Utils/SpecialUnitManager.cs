using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace VeryUsualDay.Utils
{
    public static class SpecialUnitManager
    {
        public static void Apply(
            Player player,
            List<ItemType> inventory,
            Dictionary<EffectType, byte> effects,
            float health,
            string customName,
            string customInfo,
            string broadcast,
            bool allowThreeFirearms = false)
        {

            InventoryLimitsManager.ApplySpecialUnit(
            player,
            allowThreeFirearms);

            player.ResetInventory(inventory);

            InventoryLimitsManager.RefreshAmmoLimits(player);

            player.MaxHealth = health;
            player.Health = health;
            player.CustomName = customName;
            player.CustomInfo = customInfo;

            foreach (KeyValuePair<EffectType, byte> effect in effects)
            {
                player.EnableEffect(effect.Key);
                player.ChangeEffectIntensity(effect.Key, effect.Value);
            }

            player.Broadcast(15, broadcast);
        }
    }
}