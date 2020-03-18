namespace AutoLua.Core.Logging
{
    /// <summary>
    /// 日志工厂。
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// 创建一个日志的实例。
        /// </summary>
        /// <returns>日志的实例。</returns>
        ILogger Create();
    }
}