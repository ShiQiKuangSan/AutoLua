using System;
using System.Diagnostics;
using System.Globalization;

namespace AutoLua.Core.Logging
{
    /// <summary>
    /// 日志基类。
    /// </summary>
    public abstract class LoggerBase : ILogger
    {
        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="level">日志级别。</param>
        protected LoggerBase(LogLevel level)
        {
            Level = level;
        }

        #region 属性

        /// <summary>
        /// 日志级别。
        /// </summary>
        public LogLevel Level { get; }

        #endregion

        /// <summary>
        /// 写入消息。
        /// </summary>
        /// <param name="eventType">消息类型。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="args"></param>
        protected abstract void WriteLog(TraceEventType eventType, string message, params object[] args);

        #region ILogger Members

        /// <summary>
        /// 日志信息。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="args">信息参数。</param>
        public void LogInfo(string message, params object[] args)
        {
            if (Level == LogLevel.All || Level == LogLevel.NormalAndError)
            {
                WriteLog(TraceEventType.Information, message, args);
            }
        }

        /// <summary>
        /// 非关键性错误日志信息。
        /// </summary>
        /// <param name="message">非关键性错误日志信息。</param>
        /// <param name="args">信息参数。</param>
        public void LogWarning(string message, params object[] args)
        {
            if (Level == LogLevel.All || Level == LogLevel.NormalAndError || Level == LogLevel.Error)
            {
                WriteLog(TraceEventType.Warning, message, args);
            }
        }

        /// <summary>
        /// 可恢复错误日志信息。
        /// </summary>
        /// <param name="message">可恢复错误日志信息。</param>
        /// <param name="args">信息参数。</param>
        public void LogError(string message, params object[] args)
        {
            if (Level == LogLevel.All || Level == LogLevel.NormalAndError || Level == LogLevel.Error)
            {
                WriteLog(TraceEventType.Error, message, args);
            }
        }

        /// <summary>
        /// 可恢复错误日志信息。
        /// </summary>
        /// <param name="message">可恢复错误日志信息。</param>
        /// <param name="exception">异常对象。</param>
        /// <param name="args">信息参数。</param>
        public void LogError(string message, Exception exception, params object[] args)
        {
            if (Level == LogLevel.All || Level == LogLevel.NormalAndError || Level == LogLevel.Error)
            {
                WriteLog(TraceEventType.Error,
                    $"{GetMessage(message, args)} Exception:{exception?.ToString() ?? string.Empty}");
            }
        }

        /// <summary>
        /// 调试日志信息。
        /// </summary>
        /// <param name="message">调试日志信息。</param>
        /// <param name="args">信息参数。</param>
        public void Debug(string message, params object[] args)
        {
            if (Level == LogLevel.All)
            {
                WriteLog(TraceEventType.Verbose, message, args);
            }
        }

        /// <summary>
        /// 调试日志信息。
        /// </summary>
        /// <param name="message">调试日志信息。</param>
        /// <param name="exception">异常对象。</param>
        /// <param name="args">信息参数。</param>
        public void Debug(string message, Exception exception, params object[] args)
        {
            if (Level == LogLevel.All)
            {
                WriteLog(TraceEventType.Verbose,
                    $"{GetMessage(message, args)} Exception:{exception?.ToString() ?? string.Empty}");
            }
        }

        /// <summary>
        /// 调试日志信息。
        /// </summary>
        /// <param name="item">跟踪对象。</param>
        public void Debug(object item)
        {
            if (item != null)
            {
                if (Level == LogLevel.All)
                {
                    WriteLog(TraceEventType.Verbose, item.ToString());
                }
            }
        }

        /// <summary>
        /// 致命错误信息。
        /// </summary>
        /// <param name="message">致命错误信息。</param>
        /// <param name="args">信息参数。</param>
        public void Fatal(string message, params object[] args)
        {
            if (Level == LogLevel.All || Level == LogLevel.NormalAndError || Level == LogLevel.Error)
            {
                WriteLog(TraceEventType.Critical, message, args);
            }
        }

        /// <summary>
        /// 致命错误信息。
        /// </summary>
        /// <param name="message">致命错误信息。</param>
        /// <param name="exception">异常对象。</param>
        /// <param name="args">信息参数。</param>
        public void Fatal(string message, Exception exception, params object[] args)
        {
            if (Level == LogLevel.All || Level == LogLevel.NormalAndError || Level == LogLevel.Error)
            {
                WriteLog(TraceEventType.Critical,
                    $"{GetMessage(message, args)} Exception:{exception?.ToString() ?? string.Empty}");
            }
        }

        #endregion


        #region private method

        /// <summary>
        /// 获得消息。
        /// </summary>
        /// <param name="message">消息字符串。</param>
        /// <param name="args">字符串参数。</param>
        /// <returns>消息。</returns>
        protected virtual string GetMessage(string message, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(message))
                return message;

            if (args == null || args.Length <= 0)
                return message;

            return string.Format(CultureInfo.InvariantCulture, message ?? string.Empty, args);
        }

        #endregion private method
    }
}