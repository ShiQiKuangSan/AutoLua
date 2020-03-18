namespace AutoLua.Core.Logging
{
    /// <summary>
    /// 空日志工厂。
    /// </summary>
    public class NullLoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// 日志级别。
        /// </summary>
        public LogLevel Level { get; internal set; } = LogLevel.None;

        /// <summary>
        /// 创建一个空日志的实例。
        /// </summary>
        /// <returns>空日志的实例。</returns>
        public ILogger Create() => new NullLogger();
    }
}