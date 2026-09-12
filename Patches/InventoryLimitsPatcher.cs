using System;
using System.Reflection;
using Exiled.API.Features;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items.Armor;
using VeryUsualDay.Utils;
using System.Collections.Generic;
using System.Reflection.Emit;
using InventorySystem.Searching;

namespace VeryUsualDay.Patches
{
    public static class InventoryLimitsPatcher
    {
        [ThreadStatic]
        private static Player _limitContextPlayer;

        private static readonly FieldInfo InventoryHubField =
            AccessTools.Field(typeof(Inventory), "_hub");

        public static void Patch(Harmony harmony)
        {

            PatchMethod(
                harmony,
                typeof(InventoryLimits),
                nameof(InventoryLimits.GetCategoryLimit),
                new[]
                {
                    typeof(ItemCategory),
                    typeof(ReferenceHub)
                },
                nameof(GetCategoryLimitForPlayerPrefix));

            PatchMethod(
                harmony,
                typeof(InventoryLimits),
                nameof(InventoryLimits.GetAmmoLimit),
                new[]
                {
                    typeof(ItemType),
                    typeof(ReferenceHub)
                },
                nameof(GetAmmoLimitForPlayerPrefix));

            PatchMethod(
                harmony,
                typeof(InventoryLimits),
                nameof(InventoryLimits.GetCategoryLimit),
                new[]
                {
                    typeof(BodyArmor),
                    typeof(ItemCategory)
                },
                nameof(GetCategoryLimitForArmorPrefix));

            PatchMethod(
                harmony,
                typeof(InventoryLimits),
                nameof(InventoryLimits.GetAmmoLimit),
                new[]
                {
                    typeof(BodyArmor),
                    typeof(ItemType)
                },
                nameof(GetAmmoLimitForArmorPrefix));

            MethodInfo removeExcessMethod = AccessTools.Method(
                typeof(BodyArmorUtils),
                nameof(BodyArmorUtils.RemoveEverythingExceedingLimits),
                new[]
                {
                    typeof(Inventory)
                });

            if (removeExcessMethod == null)
            {
                throw new MissingMethodException(
                    "BodyArmorUtils.RemoveEverythingExceedingLimits(Inventory) not found.");
            }

            harmony.Patch(
                removeExcessMethod,
                prefix: new HarmonyMethod(
                    typeof(InventoryLimitsPatcher),
                    nameof(RemoveEverythingExceedingLimitsPrefix)),
                postfix: new HarmonyMethod(
                    typeof(InventoryLimitsPatcher),
                    nameof(RemoveEverythingExceedingLimitsPostfix)));
            PatchPickupValidation(harmony);
        }
        private static void PatchMethod(
            Harmony harmony,
            Type declaringType,
            string methodName,
            Type[] parameters,
            string prefixName)
        {
            MethodInfo original = AccessTools.Method(
                declaringType,
                methodName,
                parameters);

            if (original == null)
            {
                throw new MissingMethodException(
                    $"{declaringType.FullName}.{methodName} with specified parameters was not found.");
            }

            MethodInfo prefix = AccessTools.Method(
                typeof(InventoryLimitsPatcher),
                prefixName);

            if (prefix == null)
            {
                throw new MissingMethodException(
                    $"Prefix {prefixName} was not found.");
            }

            harmony.Patch(
                original,
                prefix: new HarmonyMethod(prefix));
        }

        private static void RemoveEverythingExceedingLimitsPrefix(
            Inventory __0)
        {
            _limitContextPlayer = null;

            if (__0 == null || InventoryHubField == null)
                return;

            ReferenceHub hub =
                InventoryHubField.GetValue(__0) as ReferenceHub;

            if (hub != null)
                _limitContextPlayer = Player.Get(hub);
        }

        private static void RemoveEverythingExceedingLimitsPostfix()
        {
            _limitContextPlayer = null;
        }

        private static bool GetCategoryLimitForPlayerPrefix(
            ItemCategory __0,
            ReferenceHub __1,
            ref sbyte __result)
        {
            if (__1 == null)
                return true;

            Player player = Player.Get(__1);

            if (player == null)
                return true;

            if (!InventoryLimitsManager.TryGetCategoryLimit(
                    player,
                    __0,
                    out sbyte limit))
            {
                return true;
            }

            __result = limit;
            return false;
        }

        private static bool GetAmmoLimitForPlayerPrefix(
            ItemType __0,
            ReferenceHub __1,
            ref ushort __result)
        {
            if (__1 == null)
                return true;

            Player player = Player.Get(__1);

            if (player == null)
                return true;

            if (!InventoryLimitsManager.TryGetAmmoLimit(
                    player,
                    __0,
                    out ushort limit))
            {
                return true;
            }

            __result = limit;
            return false;
        }

        private static bool GetCategoryLimitForArmorPrefix(
            BodyArmor __0,
            ItemCategory __1,
            ref sbyte __result)
        {
            Player player = _limitContextPlayer;

            if (player == null &&
                __0 != null &&
                __0.Owner != null)
            {
                player = Player.Get(__0.Owner);
            }

            if (player == null)
                return true;

            if (!InventoryLimitsManager.TryGetCategoryLimit(
                    player,
                    __1,
                    out sbyte limit))
            {
                return true;
            }

            __result = limit;
            return false;
        }

        private static bool GetAmmoLimitForArmorPrefix(
            BodyArmor __0,
            ItemType __1,
            ref ushort __result)
        {
            Player player = _limitContextPlayer;

            if (player == null &&
                __0 != null &&
                __0.Owner != null)
            {
                player = Player.Get(__0.Owner);
            }

            if (player == null)
                return true;

            if (!InventoryLimitsManager.TryGetAmmoLimit(
                    player,
                    __1,
                    out ushort limit))
            {
                return true;
            }

            __result = limit;
            return false;
        }
        private static void PatchPickupValidation(Harmony harmony)
        {
            MethodInfo validateAny = AccessTools.Method(
                typeof(ItemSearchCompletor),
                "ValidateAny");

            if (validateAny == null)
            {
                throw new MissingMethodException(
                    "ItemSearchCompletor.ValidateAny was not found.");
            }

            harmony.Patch(
                validateAny,
                transpiler: new HarmonyMethod(
                    typeof(InventoryLimitsPatcher),
                    nameof(ValidateAnyTranspiler)));
        }

        private static IEnumerable<CodeInstruction> ValidateAnyTranspiler(
            IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo original = AccessTools.Method(
                typeof(InventoryLimits),
                nameof(InventoryLimits.GetCategoryLimit),
                new[]
                {
            typeof(ItemCategory),
            typeof(ReferenceHub)
                });

            MethodInfo replacement = AccessTools.Method(
                typeof(InventoryLimitsPatcher),
                nameof(GetPickupCategoryLimit));

            int replacements = 0;

            foreach (CodeInstruction instruction in instructions)
            {
                if ((instruction.opcode == OpCodes.Call ||
                     instruction.opcode == OpCodes.Callvirt) &&
                    Equals(instruction.operand, original))
                {
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = replacement;

                    replacements++;
                }

                yield return instruction;
            }

            Log.Info(
                $"[VUD Limits] ValidateAny: replaced {replacements} category limit call(s).");
        }

        private static sbyte GetPickupCategoryLimit(
            ItemCategory category,
            ReferenceHub hub)
        {
            if (hub != null)
            {
                Exiled.API.Features.Player player =
                    Exiled.API.Features.Player.Get(hub);

                if (player != null &&
                    InventoryLimitsManager.TryGetCategoryLimit(
                        player,
                        category,
                        out sbyte customLimit))
                {
                    return customLimit;
                }
            }

            return InventoryLimits.GetCategoryLimit(
                category,
                hub);
        }
    }
}