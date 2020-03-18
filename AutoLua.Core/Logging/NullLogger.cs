using System;

namespace AutoLua.Core.Logging
{
    /// <summary>
    /// 空日志。
    /// </summary>
    public class NullLogger : ILogger
    {
        /// <summary>
        /// 空日志对象的实例。
        /// </summary>
        public static ILogger Instance { get; } = new NullLogger();

        /// <summary>
        /// 调试日志信息。
        /// </summary>
        /// <param name="message">调试日志信息。</param>
        /// <param name="args">信息参数。</param>
        public void Debug(string message, params object[] args)
        {
        }

        /// <summary>
        /// 调试日志信息。
        /// </summary>
        /// <param name="message">调试日志信息。</param>
        /// <param name="exception">异常对象。</param>
        /// <param name="args">信息参数。</param>
        public void Debug(string message, Exception exception, params object[] args)
        {
        }

        /// <summary>
        /// 调试日志信息。
        /// </summary>
        /// <param name="item">跟踪对象。</param>
        public void Debug(object item)
        {
        }

        /// <summary>
        /// 致命错误信息。
        /// </summary>
        /// <param name="message">致命错误信息。</param>
        /// <param name="args">信息参数。</param>
        public void Fatal(string message, params object[] args)
        {
        }

        /// <summary>
        /// 致命错误信息。
        /// </summary>
        /// <param name="message">致命错误信息。</param>
        /// <param name="exception">异常对象。</param>
        /// <param name="args">信息参数。</param>
        public void Fatal(string message, Exception exception, params object[] args)
        {
        }

        /// <summary>
        /// 日志信息。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="args">信息参数。</param>
        public void LogInfo(string message, params object[] args)
        {
        }

        /// <summary>
        /// 非关键性错误日志信息。
        /// </summary>
        /// <param name="message">非关键性错误日志信息。</param>
        /// <param name="args">信息参数。</param>
        public void LogWarning(string message, params object[] args)
        {
        }

        /// <summary>
        /// 可恢复错误日志信息。
        /// </summary>
        /// <param name="message">可恢复错误日志信息。</param>
        /// <param name="args">信息参数。</param>
        public void LogError(string message, params object[] args)
        {
        }

        /// <summary>
        /// 可恢复错误日志信息。
        /// </summary>
        /// <param name="message">可恢复错误日志信息。</param>
        /// <param name="exception">异常对象。</param>
        /// <param name="args">信息参数。</param>
        public void LogError(string message, Exception exception, params object[] args)
        {
        }
    }
}