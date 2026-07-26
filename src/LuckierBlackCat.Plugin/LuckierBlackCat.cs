using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LuckierBlackCat.Core;
using LuckierBlackCat.Patching;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LuckierBlackCat
{
    /// <summary>
    /// Luckier Black Cat 插件主类
    /// 这是一个用于游戏 Elin 的 BepInEx 插件，主要功能是增强黑猫的舔舐效果
    /// </summary>
    [BepInPlugin("com.travellerse.plugins.LuckierBlackCat", "Luckier Black Cat", "2.0.0.0")]
    [BepInProcess("Elin.exe")]
    public class LuckierBlackCat : BaseUnityPlugin
    {
        #region 插件信息常量

        /// <summary>
        /// 插件的唯一标识符
        /// </summary>
        public const string PLUGIN_GUID = "com.travellerse.plugins.LuckierBlackCat";

        /// <summary>
        /// 插件名称
        /// </summary>
        public const string PLUGIN_NAME = "Luckier Black Cat";

        /// <summary>
        /// 插件版本
        /// </summary>
        public const string PLUGIN_VERSION = "2.0.0.0";

        #endregion

        #region 私有字段

        /// <summary>
        /// 配置文件实例
        /// </summary>
        private ConfigFile _configFile;

        /// <summary>
        /// Harmony 实例
        /// </summary>
        private Harmony _harmony;

        #endregion

        #region 插件生命周期

        /// <summary>
        /// 插件初始化方法，在插件加载时调用
        /// 设置配置项并根据配置启用相应的 Harmony 补丁
        /// </summary>
        public void Awake()
        {
            try
            {
                // 初始化日志记录器
                Utils.Logger.Initialize(base.Logger);
                Utils.Logger.LogInfo(PLUGIN_NAME + " v" + PLUGIN_VERSION + " is loading...");

                // 初始化配置文件
                InitializeConfig();

                // 初始化配置管理器
                ConfigManager.Initialize(_configFile, base.Logger);

                // 验证配置
                ConfigManager.ValidateConfig(base.Logger);

                // 创建 Harmony 实例
                _harmony = new Harmony(PLUGIN_GUID);

                // 应用补丁
                IReadOnlyList<PatchFeatureResult> patchResults =
                    PatchManager.ApplyPatches(_harmony, base.Logger);

                bool hasIncompatibleFeature = false;
                foreach (PatchFeatureResult result in patchResults)
                {
                    if (result.Status == PatchFeatureStatus.Incompatible)
                    {
                        hasIncompatibleFeature = true;
                        break;
                    }
                }

                if (hasIncompatibleFeature)
                {
                    Utils.Logger.LogWarning(PLUGIN_NAME + " loaded with incompatible features disabled.");
                }
                else
                {
                    Utils.Logger.LogInfo(PLUGIN_NAME + " loaded successfully!");
                }
            }
            catch (System.Exception ex)
            {
                Utils.Logger.LogError("Failed to load " + PLUGIN_NAME + ":\n" + ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// 插件销毁时调用
        /// </summary>
        public void OnDestroy()
        {
            try
            {
                // 移除所有 Harmony 补丁
                if (_harmony != null)
                {
                    _harmony.UnpatchSelf();
                }
                Utils.Logger.LogInfo(PLUGIN_NAME + " unloaded successfully.");
            }
            catch (System.Exception ex)
            {
                Utils.Logger.LogError("Error during " + PLUGIN_NAME + " unload: " + ex.Message);
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化配置文件
        /// </summary>
        private void InitializeConfig()
        {
            string configPath = Path.Combine(Paths.ConfigPath, "LuckierBlackCat.cfg");
            _configFile = new ConfigFile(configPath, true);
            Utils.Logger.LogInfo("Configuration file path: " + configPath);

            try
            {
                FieldInfo configField = typeof(BaseUnityPlugin).GetField(
                    "<Config>k__BackingField",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                if (configField == null)
                {
                    Utils.Logger.LogWarning(
                        "BepInEx Config field was not found; Mod Config GUI integration is unavailable.");
                    return;
                }

                configField.SetValue(this, _configFile);
            }
            catch (System.Exception ex)
            {
                Utils.Logger.LogWarning(
                    "Could not expose the configuration to Mod Config GUI: " + ex.Message);
            }
        }

        #endregion
    }
}
