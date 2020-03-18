using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using AutoLua.Core.Logging;

namespace AutoLua.Droid.Utils.Logging
{
    /// <summary>
    /// 文本日志记录器。
    /// </summary>
    public class TxtLogger : LoggerBase
    {
        private static readonly object _lockObject = new object();

        public TxtLogger(LogLevel level) : base(level)
        {
        }

        /// <summary>
        /// 写入日志。
        /// </summary>
        /// <param name="eventType">日志类型。</param>
        /// <param name="message">日志信息。</param>
        /// <param name="args">日志参数。</param>
        protected override void WriteLog(TraceEventType eventType, string message, params object[] args)
        {
            var path = AppApplication.LogPath;

            try
            {
                //如果日志目录不存在就创建。
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //获取当前系统时间。
                var time = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}";

                //用日期对日志文件命名。
                var filename = path + $"/{DateTime.Now:yyyy-MM-dd-HH}.log";

                WirteFile(filename, $"{time}: {message}");
            }
            catch (Exception)
            {

            }
        }

        private void WirteFile(string path, string message)
        {
            try
            {
                lock (_lockObject)
                {
                    using var streamWriter = System.IO.File.AppendText(path);
                    var builder = new StringBuilder();
                    builder.AppendLine(message);
                    builder.AppendLine();
                    builder.AppendLine();
                    builder.AppendLine();
                    streamWriter.Write(builder);
                }
            }
            catch (Exception exception)
            {
                var errorBuilder = new StringBuilder();

                errorBuilder.AppendLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: 写入日志：{message} 过程中出现异常");
                errorBuilder.AppendLine(exception.Message);
                errorBuilder.AppendLine(exception.StackTrace);

                lock (_lockObject)
                {
                    using var streamWriter = System.IO.File.AppendText(path);
                    streamWriter.Write(message);
                }
            }
        }
    }
}