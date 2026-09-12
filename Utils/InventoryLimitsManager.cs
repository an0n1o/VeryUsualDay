using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace VeryUsualDay.Utils
{
    public static class InventoryLimitsManager
    {
        private const sbyte UnlimitedCategoryLimit = 8;

        private sealed class LimitsProfile
        {
            public readonly Dictionary<ItemCategory, sbyte> CategoryLimits =
                new Dictionary<ItemCategory, sbyte>();

            public readonly Dictionary<ItemType, ushort> AmmoLimits =
                new Dictionary<ItemType, ushort>();
        }

        private static readonly Dictionary<int, LimitsProfile> PlayerLimits =
            new Dictionary<int, LimitsProfile>();

        private static readonly HashSet<int> ArmorAmmoPlayers =
    new HashSet<int>();

        public static void Apply(Player player, string department, string position)
        {
            if (player == null)
                return;

            Clear(player);

            switch (department)
            {
                case "НС":
                    ApplyScientificLimits(player, position);
                    break;

                case "СБ":
                    EnableArmorAmmoLimits(player);
                    break;

                case "ГОР":
                    EnableArmorAmmoLimits(player);
                    SetCategoryLimit(player,
                    ItemCategory.SpecialWeapon,
                    2);
                    break;
            }
        }
        public static void ApplySpecialUnit(
            Player player,
            bool allowThreeFirearms = false)
        {
            if (player == null)
                return;

            Clear(player);

            EnableArmorAmmoLimits(player);

            SetCategoryLimit(
                player,
                ItemCategory.SpecialWeapon,
                2);

            if (allowThreeFirearms)
            {
                SetCategoryLimit(
                    player,
                    ItemCategory.Firearm,
                    3);
            }
        }

        public static void ApplyBuo(Player player)
        {
            ApplySpecialUnit(player);
        }
        public static void FillAmmoToLimit(
    Player player,
    params AmmoType[] ammoTypes)
        {
            if (player == null || ammoTypes == null)
                return;

            HashSet<AmmoType> processedAmmo = new HashSet<AmmoType>();

            foreach (AmmoType ammoType in ammoTypes)
            {
                if (!processedAmmo.Add(ammoType))
                    continue;

                ItemType ammoItem;

                switch (ammoType)
                {
                    case AmmoType.Nato9:
                        ammoItem = ItemType.Ammo9x19;
                        break;

                    case AmmoType.Nato556:
                        ammoItem = ItemType.Ammo556x45;
                        break;

                    case AmmoType.Nato762:
                        ammoItem = ItemType.Ammo762x39;
                        break;

                    case AmmoType.Ammo12Gauge:
                        ammoItem = ItemType.Ammo12gauge;
                        break;

                    case AmmoType.Ammo44Cal:
                        ammoItem = ItemType.Ammo44cal;
                        break;

                    default:
                        continue;
                }

                if (TryGetAmmoLimit(player, ammoItem, out ushort limit))
                    player.AddAmmo(ammoType, limit);
            }
        }

        private static void ApplyScientificLimits(Player player, string position)
        {
            switch (position)
            {
                case "Медик":
                    SetCategoryLimit(
                        player,
                        ItemCategory.Medical,
                        UnlimitedCategoryLimit);
                    break;

                case "Исследователь":
                    SetCategoryLimit(
                        player,
                        ItemCategory.SCPItem,
                        UnlimitedCategoryLimit);
                    break;

                case "Инженер":
                    SetCategoryLimit(
                        player,
                        ItemCategory.Radio,
                        8);
                    break;
            }
        }

        private static void EnableArmorAmmoLimits(Player player)
        {
            ArmorAmmoPlayers.Add(player.Id);
            RefreshAmmoLimits(player);
        }

        public static void RefreshAmmoLimits(Player player)
        {
            if (player == null || !ArmorAmmoPlayers.Contains(player.Id))
                return;

            ResetAmmoLimitsOnly(player);
            if (player.CurrentArmor == null)
                return;

            switch (player.CurrentArmor.Type)
            {
                case ItemType.ArmorLight:
                    // 90 + 30 от лёгкой брони = 120.
                    SetAmmoLimit(
                        player,
                        AmmoType.Nato9,
                        ItemType.Ammo9x19,
                        90);
                    break;

                case ItemType.ArmorCombat:
                    // 270 + 130 = 400.
                    SetAmmoLimit(
                        player,
                        AmmoType.Nato9,
                        ItemType.Ammo9x19,
                        270);

                    // 320 + 80 = 400.
                    SetAmmoLimit(
                        player,
                        AmmoType.Nato556,
                        ItemType.Ammo556x45,
                        320);

                    SetAmmoLimit(
                        player,
                        AmmoType.Nato762,
                        ItemType.Ammo762x39,
                        320);

                    // 160 + 40 = 200.
                    SetAmmoLimit(
                        player,
                        AmmoType.Ammo12Gauge,
                        ItemType.Ammo12gauge,
                        160);

                    // 170 + 30 = 200.
                    SetAmmoLimit(
                        player,
                        AmmoType.Ammo44Cal,
                        ItemType.Ammo44cal,
                        170);
                    break;

                case ItemType.ArmorHeavy:
                    // 330 + 170 = 500.
                    SetAmmoLimit(
                        player,
                        AmmoType.Nato9,
                        ItemType.Ammo9x19,
                        330);

                    // 340 + 160 = 500.
                    SetAmmoLimit(
                        player,
                        AmmoType.Nato556,
                        ItemType.Ammo556x45,
                        340);

                    SetAmmoLimit(
                        player,
                        AmmoType.Nato762,
                        ItemType.Ammo762x39,
                        340);

                    // 240 + 60 = 300.
                    SetAmmoLimit(
                        player,
                        AmmoType.Ammo12Gauge,
                        ItemType.Ammo12gauge,
                        240);

                    // 250 + 50 = 300.
                    SetAmmoLimit(
                        player,
                        AmmoType.Ammo44Cal,
                        ItemType.Ammo44cal,
                        250);
                    break;
            }
        }
        private static void ResetAmmoLimitsOnly(Player player)
        {
            if (!PlayerLimits.TryGetValue(player.Id, out LimitsProfile profile))
                return;

            foreach (ItemType ammoItem in profile.AmmoLimits.Keys)
            {
                AmmoType ammoType = GetAmmoType(ammoItem);

                if (ammoType != AmmoType.None &&
                    player.HasCustomAmmoLimit(ammoType))
                {
                    player.ResetAmmoLimit(ammoType);
                }
            }

            profile.AmmoLimits.Clear();
        }

        private static LimitsProfile GetOrCreateProfile(Player player)
        {
            LimitsProfile profile;

            if (!PlayerLimits.TryGetValue(player.Id, out profile))
            {
                profile = new LimitsProfile();
                PlayerLimits.Add(player.Id, profile);
            }

            return profile;
        }

        private static void SetCategoryLimit(
            Player player,
            ItemCategory category,
            sbyte limit)
        {
            LimitsProfile profile = GetOrCreateProfile(player);
            profile.CategoryLimits[category] = limit;

            if (category == ItemCategory.Medical ||
                category == ItemCategory.SCPItem)
            {
                player.SetCategoryLimit(category, limit);
            }
        }

        private static void SetAmmoLimit(
            Player player,
            AmmoType ammoType,
            ItemType itemType,
            ushort limit)
        {
            LimitsProfile profile = GetOrCreateProfile(player);
            profile.AmmoLimits[itemType] = limit;

            player.SetAmmoLimit(ammoType, limit);
        }

        public static bool TryGetCategoryLimit(
            Player player,
            ItemCategory category,
            out sbyte limit)
        {
            limit = 0;

            if (player == null)
                return false;

            LimitsProfile profile;

            return PlayerLimits.TryGetValue(player.Id, out profile) &&
                   profile.CategoryLimits.TryGetValue(category, out limit);
        }

        public static bool TryGetAmmoLimit(
    Player player,
    ItemType ammoType,
    out ushort limit)
        {
            limit = 0;

            if (player == null ||
                !ArmorAmmoPlayers.Contains(player.Id) ||
                player.CurrentArmor == null)
            {
                return false;
            }

            switch (player.CurrentArmor.Type)
            {
                case ItemType.ArmorLight:
                    if (ammoType == ItemType.Ammo9x19)
                    {
                        limit = 120;
                        return true;
                    }
                    return false;

                case ItemType.ArmorCombat:
                    switch (ammoType)
                    {
                        case ItemType.Ammo9x19:
                        case ItemType.Ammo556x45:
                        case ItemType.Ammo762x39:
                            limit = 400;
                            return true;

                        case ItemType.Ammo12gauge:
                        case ItemType.Ammo44cal:
                            limit = 200;
                            return true;
                    }

                    return false;

                case ItemType.ArmorHeavy:
                    switch (ammoType)
                    {
                        case ItemType.Ammo9x19:
                        case ItemType.Ammo556x45:
                        case ItemType.Ammo762x39:
                            limit = 500;
                            return true;

                        case ItemType.Ammo12gauge:
                        case ItemType.Ammo44cal:
                            limit = 300;
                            return true;
                    }

                    return false;

                default:
                    return false;
            }
        }

        public static void Clear(Player player)
        {
            if (player == null)
                return;
            ArmorAmmoPlayers.Remove(player.Id);
            LimitsProfile profile;

            if (!PlayerLimits.TryGetValue(player.Id, out profile))
                return;

            foreach (ItemCategory category in profile.CategoryLimits.Keys)
            {
                if ((category == ItemCategory.Medical ||
                     category == ItemCategory.SCPItem) &&
                    player.HasCustomCategoryLimit(category))
                {
                    player.ResetCategoryLimit(category);
                }
            }

            foreach (ItemType ammoItem in profile.AmmoLimits.Keys)
            {
                AmmoType ammoType = GetAmmoType(ammoItem);

                if (ammoType != AmmoType.None &&
                    player.HasCustomAmmoLimit(ammoType))
                {
                    player.ResetAmmoLimit(ammoType);
                }
            }

            PlayerLimits.Remove(player.Id);
        }

        public static void Forget(Player player)
        {
            if (player == null)
                return;
            ArmorAmmoPlayers.Remove(player.Id);
            PlayerLimits.Remove(player.Id);
        }

        public static void ClearAll()
        {
            foreach (Player player in Player.List)
                Clear(player);

            PlayerLimits.Clear();
        }

        private static AmmoType GetAmmoType(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Ammo12gauge:
                    return AmmoType.Ammo12Gauge;

                case ItemType.Ammo44cal:
                    return AmmoType.Ammo44Cal;

                case ItemType.Ammo9x19:
                    return AmmoType.Nato9;

                case ItemType.Ammo556x45:
                    return AmmoType.Nato556;

                case ItemType.Ammo762x39:
                    return AmmoType.Nato762;

                default:
                    return AmmoType.None;
            }
        }
    }
}