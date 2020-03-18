using System;

namespace AutoLua.Core.Logging
{
    /// <summary>
    /// 日志接口。
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 调试日志信息。
        /// </summary>
        /// <param name="message">调试日志信息。</param>
        /// <param name="args">信息参数。</param>
        void Debug(string message, params object[] args);

        /// <summary>
        /// 调试日志信息。
        /// </summary>
        /// <param name="message">调试日志信息。</param>
        /// <param name="exception">异常对象。</param>
        /// <param name="args">信息参数。</param>
        void Debug(string message, Exception exception, params object[] args);

        /// <summary>
        /// 调试日志信息。
        /// </summary>
        /// <param name="item">跟踪对象。</param>
        void Debug(object item);

        /// <summary>
        /// 致命错误信息。
        /// </summary>
        /// <param name="message">致命错误信息。</param>
        /// <param name="args">信息参数。</param>
        void Fatal(string message, params object[] args);

        /// <summary>
        /// 致命错误信息。
        /// </summary>
        /// <param name="message">致命错误信息。</param>
        /// <param name="exception">异常对象。</param>
        /// <param name="args">信息参数。</param>
        void Fatal(string message, Exception exception, params object[] args);

        /// <summary>
        /// 日志信息。
        /// </summary>
        /// <param name="message">日志信息。</param>
        /// <param name="args">信息参数。</param>
        void LogInfo(string message, params object[] args);

        /// <summary>
        /// 非关键性错误日志信息。
        /// </summary>
        /// <param name="message">非关键性错误日志信息。</param>
        /// <param name="args">信息参数。</param>
        void LogWarning(string message, params object[] args);

        /// <summary>
        /// 可恢复错误日志信息。
        /// </summary>
        /// <param name="message">可恢复错误日志信息。</param>
        /// <param name="args">信息参数。</param>
        void LogError(string message, params object[] args);

        /// <summary>
        /// 可恢复错误日志信息。
        /// </summary>
        /// <param name="message">可恢复错误日志信息。</param>
        /// <param name="exception">异常对象。</param>
        /// <param name="args">信息参数。</param>
        void LogError(string message, Exception exception, params object[] args);
    }
}