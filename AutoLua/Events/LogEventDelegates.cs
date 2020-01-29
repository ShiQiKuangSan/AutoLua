using System;

namespace AutoLua.Events
{
    /// <summary>
    /// 日志事件。
    /// </summary>
    public class LogEventDelegates
    {
        /// <summary>
        /// 日志事件。
        /// </summary>
        public event EventHandler<LogEventArgs> Log;

        private readonly static object Lock = new object();

        private static LogEventDelegates instance = null;

        public static LogEventDelegates Instance
        {
            get
            {
                lock (Lock)
                {
                    if (instance == null) instance = new LogEventDelegates();

                    return instance;
                }
            }
        }

        /// <summary>
        /// 发送日志。
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnLog(LogEventArgs e)
        {
            Log?.Invoke(this, e);
        }
    }
}
