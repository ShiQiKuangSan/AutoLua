namespace AutoLua.Core.Logging
{
    /// <summary>
    /// 日志等级
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 不输出日志。
        /// </summary>
        None = 0,

        /// <summary>
        /// 只输出错误日志。
        /// </summary>
        Error = 1,

        /// <summary>
        /// 只输出通常日志和错误日志。
        /// </summary>
        NormalAndError = 2,

        /// <summary>
        /// 输出所有日志。
        /// </summary>
        All = 3
    }
}