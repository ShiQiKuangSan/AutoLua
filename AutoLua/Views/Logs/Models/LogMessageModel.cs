using System;
using Xamarin.Forms;

namespace AutoLua.Views.Logs.Models
{
    public class LogMessageModel
    {
        /// <summary>
        /// 日志信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 日志颜色
        /// </summary>
        public Color TextColor { get; set; } = Color.Black;

        public LogMessageModel(string type,string message)
        {
            var time = DateTime.Now.ToString("HH:mm:ss");

            Message = $"{type} -->> {time} -->>         {message}";
        }
    }
}
