using AutoLua.Core.Logging;

namespace AutoLua.Droid.Utils.Logging
{
    public sealed class TxtLoggerFactory : ILoggerFactory
    {
        private static readonly object _lockObject = new object();

        private static ILogger _logger = null;

        /// <summary>
        /// 日志级别。
        /// </summary>
        public LogLevel Level { get; set; } = LogLevel.All;

        /// <summary>
        /// 创建一个文本日志的实例。
        /// </summary>
        /// <returns>文本日志的实例。</returns>
        public ILogger Create()
        {
            if (_logger != null)
                return _logger;

            lock (_lockObject)
            {
                if (_logger != null)
                    return _logger;

                _logger = new TxtLogger(Level);
            }

            return _logger;
        }
    }
}