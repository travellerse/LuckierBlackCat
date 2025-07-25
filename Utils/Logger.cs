using BepInEx.Logging;

namespace LuckierBlackCat.Utils
{
    /// <summary>
    /// 日志工具类
    /// 提供统一的日志记录接口
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// 内部日志记录器实例
        /// </summary>
        private static ManualLogSource _logger;

        /// <summary>
        /// 初始化日志记录器
        /// </summary>
        /// <param name="logger">BepInEx 日志记录器实例</param>
        public static void Initialize(ManualLogSource logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 记录信息级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public static void LogInfo(string message)
        {
            _logger?.LogInfo(message);
        }

        /// <summary>
        /// 记录警告级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public static void LogWarning(string message)
        {
            _logger?.LogWarning(message);
        }

        /// <summary>
        /// 记录错误级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public static void LogError(string message)
        {
            _logger?.LogError(message);
        }

        /// <summary>
        /// 记录调试级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public static void LogDebug(string message)
        {
            _logger?.LogDebug(message);
        }

        /// <summary>
        /// 检查日志记录器是否已初始化
        /// </summary>
        /// <returns>是否已初始化</returns>
        public static bool IsInitialized => _logger != null;
    }
}
