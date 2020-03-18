using System;

namespace AutoLua.Core.Logging
{
    /// <summary>
    /// 日志工厂
    /// </summary>
    public sealed class LoggerFactory
    {
        /// <summary>
        /// 日志工厂的实例。
        /// </summary>
        private static readonly LoggerFactory Instance = new LoggerFactory();

        /// <summary>
        /// 当前日志工厂。
        /// </summary>
        public static ILoggerFactory Current => Instance.InnerCurrent;

        /// <summary>
        /// 当前日志工厂。
        /// </summary>
        private ILoggerFactory InnerCurrent { get; set; } = new NullLoggerFactory();

        /// <summary>
        /// 设置当前使用的日志方式。
        /// </summary>
        /// <param name="logFactory">日志工厂。</param>
        public static void SetLoggerFactory(ILoggerFactory logFactory) => Instance.InnerSetLoggerFactory(logFactory);

        /// <summary>
        /// 设置当前使用的日志方式。
        /// </summary>
        /// <param name="logFactory">日志工厂。</param>
        private void InnerSetLoggerFactory(ILoggerFactory logFactory)
        {
            InnerCurrent = logFactory ?? throw new ArgumentNullException(nameof(logFactory));
        }
    }
}