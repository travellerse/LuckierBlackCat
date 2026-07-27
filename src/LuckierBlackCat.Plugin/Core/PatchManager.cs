using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using LuckierBlackCat.Patches;
using LuckierBlackCat.Patching;

namespace LuckierBlackCat.Core
{
    public static class PatchManager
    {
        public static IReadOnlyList<PatchFeatureResult> ApplyPatches(
            Harmony harmony,
            ManualLogSource logger)
        {
            var features = new[]
            {
                new PatchFeature(
                    "distance",
                    ConfigManager.EnableLickWithoutDist.Value,
                    ValidateDistance,
                    () => ApplyDistance(harmony)),
                new PatchFeature(
                    "pickup",
                    ConfigManager.EnableLickWhenPick.Value,
                    ValidatePickup,
                    () => harmony.PatchAll(typeof(CharaPickPatch))),
                new PatchFeature(
                    "prayer",
                    ConfigManager.EnableLickWhenPray.Value,
                    ValidatePrayer,
                    () => harmony.PatchAll(typeof(ActPrayTryPrayPatch))),
                new PatchFeature(
                    "enchantment",
                    ConfigManager.EnableLickEnchant.Value,
                    ThingTryLickEnchantPatch.Validate,
                    () => harmony.PatchAll(typeof(ThingTryLickEnchantPatch))),
            };

            var results = PatchCatalog.Apply(features, harmony.UnpatchSelf);
            foreach (var result in results)
            {
                LogResult(logger, result);
            }

            return results;
        }

        private static void ValidateDistance()
        {
            ThingGenTryLickChestPatch.Validate();
            SpawnLootPatch.Validate();
        }

        private static void ApplyDistance(Harmony harmony)
        {
            harmony.PatchAll(typeof(ThingGenTryLickChestPatch));
            harmony.PatchAll(typeof(SpawnLootPatch));
        }

        private static void ValidatePickup()
        {
            RequireMethod(
                typeof(Chara),
                "Pick",
                typeof(Thing),
                false,
                typeof(Thing),
                typeof(bool),
                typeof(bool));
        }

        private static void ValidatePrayer()
        {
            RequireMethod(
                typeof(ActPray),
                "TryPray",
                typeof(bool),
                true,
                typeof(Chara),
                typeof(bool));
        }

        private static MethodInfo RequireMethod(
            Type declaringType,
            string name,
            Type returnType,
            bool isStatic,
            params Type[] parameterTypes)
        {
            var method = AccessTools.Method(declaringType, name, parameterTypes);
            if (method == null || method.ReturnType != returnType || method.IsStatic != isStatic)
            {
                throw new MissingMethodException(
                    "Missing or incompatible target: "
                    + declaringType.Name
                    + "."
                    + name
                    + ".");
            }

            return method;
        }

        private static void LogResult(ManualLogSource logger, PatchFeatureResult result)
        {
            string message = "Patch feature " + result.Name + ": " + result.Status + ". " + result.Detail;
            if (result.Status == PatchFeatureStatus.Incompatible)
            {
                logger.LogError(message);
            }
            else if (result.Status == PatchFeatureStatus.Disabled)
            {
                logger.LogInfo(message);
            }
            else
            {
                logger.LogInfo(message);
            }
        }
    }
}
