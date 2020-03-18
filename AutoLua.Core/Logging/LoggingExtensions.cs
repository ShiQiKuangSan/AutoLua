using System;

namespace AutoLua.Core.Logging
{
    /// <summary>
    /// 日志扩展。
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// 错误日志。
        /// </summary>
        /// <param name="logger">日志对象。</param>
        /// <param name="exception">异常信息对象。</param>
        /// <param name="format">可恢复错误日志信息。</param>
        /// <param name="args">信息参数。</param>
        public static void Error(this ILogger logger, Exception exception, string format, params object[] args) => logger.LogError(format, exception, args);

        /// <summary>
        /// 致命错误日志。
        /// </summary>
        /// <param name="logger">日志对象。</param>
        /// <param name="exception">异常信息对象。</param>
        /// <param name="format">致命错误信息。</param>
        /// <param name="args">信息参数。</param>
        public static void Fatal(this ILogger logger, Exception exception, string format, params object[] args) => logger.Fatal(format, exception, args);
    }
}