using BepInEx.Logging;
using HarmonyLib;
using LuckierBlackCat.Patches;

namespace LuckierBlackCat.Core
{
    /// <summary>
    /// 补丁管理器
    /// 负责根据配置应用相应的 Harmony 补丁
    /// </summary>
    public static class PatchManager
    {
        /// <summary>
        /// 应用所有补丁
        /// </summary>
        /// <param name="harmony">Harmony 实例</param>
        /// <param name="logger">日志记录器</param>
        public static void ApplyPatches(Harmony harmony, ManualLogSource logger)
        {
            logger.LogInfo("Applying patches based on configuration...");

            // 根据配置启用相应功能的补丁
            if (ConfigManager.EnableLickWithoutDist.Value)
            {
                ApplyDistancePatches(harmony, logger);
            }

            if (ConfigManager.EnableLickWhenPick.Value)
            {
                ApplyPickupPatches(harmony, logger);
            }

            if (ConfigManager.EnableLickWhenPray.Value)
            {
                ApplyPrayerPatches(harmony, logger);
            }

            if (ConfigManager.EnableLickEnchant.Value)
            {
                ApplyEnchantmentPatches(harmony, logger);
            }

            logger.LogInfo("All patches applied successfully.");
        }

        /// <summary>
        /// 应用无距离限制相关的补丁
        /// </summary>
        /// <param name="harmony">Harmony 实例</param>
        /// <param name="logger">日志记录器</param>
        private static void ApplyDistancePatches(Harmony harmony, ManualLogSource logger)
        {
            harmony.PatchAll(typeof(ThingGenTryLickChestPatch));
            harmony.PatchAll(typeof(SpawnLootPatch));
            logger.LogInfo("Distance limit patches applied.");
        }

        /// <summary>
        /// 应用拾取装备时舔舐的补丁
        /// </summary>
        /// <param name="harmony">Harmony 实例</param>
        /// <param name="logger">日志记录器</param>
        private static void ApplyPickupPatches(Harmony harmony, ManualLogSource logger)
        {
            harmony.PatchAll(typeof(CharaPickPatch));
            logger.LogInfo("Equipment pickup patches applied.");
        }

        /// <summary>
        /// 应用祈祷时舔舐的补丁
        /// </summary>
        /// <param name="harmony">Harmony 实例</param>
        /// <param name="logger">日志记录器</param>
        private static void ApplyPrayerPatches(Harmony harmony, ManualLogSource logger)
        {
            harmony.PatchAll(typeof(ActPrayTryPrayPatch));
            logger.LogInfo("Prayer patches applied.");
        }

        /// <summary>
        /// 应用增强舔舐附魔效果的补丁
        /// </summary>
        /// <param name="harmony">Harmony 实例</param>
        /// <param name="logger">日志记录器</param>
        private static void ApplyEnchantmentPatches(Harmony harmony, ManualLogSource logger)
        {
            harmony.PatchAll(typeof(ThingTryLickEnchantPatch));
            harmony.PatchAll(typeof(ThingGetEnchantPatch));
            harmony.PatchAll(typeof(ThingAddEnchantPatch));
            logger.LogInfo("Enhanced enchantment patches applied.");
        }
    }
}
