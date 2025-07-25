using BepInEx.Configuration;
using BepInEx.Logging;

namespace LuckierBlackCat.Core
{
    /// <summary>
    /// 配置管理器
    /// 负责管理插件的所有配置项，提供统一的配置访问接口
    /// </summary>
    public static class ConfigManager
    {
        #region 配置项定义

        /// <summary>
        /// 配置项：启用无距离限制的舔舐功能
        /// </summary>
        public static ConfigEntry<bool> EnableLickWithoutDist { get; private set; }

        /// <summary>
        /// 配置项：启用拾取装备时的舔舐功能
        /// </summary>
        public static ConfigEntry<bool> EnableLickWhenPick { get; private set; }

        /// <summary>
        /// 配置项：启用祈祷时的舔舐功能
        /// </summary>
        public static ConfigEntry<bool> EnableLickWhenPray { get; private set; }

        /// <summary>
        /// 配置项：启用增强版舔舐附魔功能
        /// </summary>
        public static ConfigEntry<bool> EnableLickEnchant { get; private set; }

        /// <summary>
        /// 配置项：附魔增强倍数
        /// </summary>
        public static ConfigEntry<int> EnchantTimes { get; private set; }

        /// <summary>
        /// 配置项：是否需要舔舐能力才能发挥作用
        /// </summary>
        public static ConfigEntry<bool> RequireLickAbility { get; private set; }

        #endregion

        #region 初始化方法

        /// <summary>
        /// 初始化配置管理器
        /// </summary>
        /// <param name="configFile">BepInEx 配置文件实例</param>
        /// <param name="logger">日志记录器</param>
        public static void Initialize(ConfigFile configFile, ManualLogSource logger)
        {
            logger.LogInfo("Initializing configuration...");

            // 绑定配置项，设置默认值和描述
            EnableLickWithoutDist = configFile.Bind("Settings", "EnableLickWithoutDist", true,
                "Enable lick without distance limit.");

            EnableLickWhenPick = configFile.Bind("Settings", "EnableLickWhenPick", true,
                "Enable lick when pick up equipment.");

            EnableLickWhenPray = configFile.Bind("Settings", "EnableLickWhenPray", true,
                "Enable lick when pray.");

            EnableLickEnchant = configFile.Bind("Settings", "EnableLickEnchant", true,
                "Enhanced the black cat's licking effect based on the amount of [Black Cat's Saliva].");

            EnchantTimes = configFile.Bind("Settings", "EnchantTimes", 1,
                "The times of enhance");

            RequireLickAbility = configFile.Bind("Settings", "RequireLickAbility", true,
                "Whether the mod requires lick ability to work. If set to false, the mod will work even without black cats with lick ability.");

            logger.LogInfo("Configuration initialized successfully.");
        }

        #endregion

        #region 配置验证方法

        /// <summary>
        /// 验证配置的合理性
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public static void ValidateConfig(ManualLogSource logger)
        {
            // 验证附魔倍数的合理性
            if (EnchantTimes.Value < 0)
            {
                logger.LogWarning(string.Format("EnchantTimes value is negative ({0}), setting to 0.", EnchantTimes.Value));
                EnchantTimes.Value = 0;
            }
            else if (EnchantTimes.Value > 10)
            {
                logger.LogWarning(string.Format("EnchantTimes value is very high ({0}), this may cause balance issues.", EnchantTimes.Value));
            }
        }

        #endregion
    }
}
