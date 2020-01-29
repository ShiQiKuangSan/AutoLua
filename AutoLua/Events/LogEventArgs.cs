using System;
using Xamarin.Forms;

namespace AutoLua.Events
{
    /// <summary>
    /// 日志事件参数。
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        public LogEventArgs(string type,string message, Color textColor)
        {
            Type = type;
            Message = message;
            TextColor = textColor;
        }

        /// <summary>
        /// 日志类型
        /// </summary>
        public string Type { get; set; }

        public string Message { get; set; }

        public Color TextColor { get; set; } = Color.Black;
    }
}
