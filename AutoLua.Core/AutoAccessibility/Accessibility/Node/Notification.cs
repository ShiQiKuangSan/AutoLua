using System;
using Android.OS;

namespace AutoLua.Core.AutoAccessibility.Accessibility.Node
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class Notification : Android.App.Notification
    {
        /// <summary>
        /// 获取通知的内容。
        /// </summary>
        public readonly string text;

        /// <summary>
        /// 获取通知的标题。
        /// </summary>
        public readonly string title;

        /// <summary>
        /// 包名.
        /// </summary>
        public readonly string packageName;

        public Notification(string packageName)
        {
            text = Extras.GetString(ExtraText);
            title = Extras.GetString(ExtraTitle);
            this.packageName = packageName;
        }

        /// <summary>
        /// 点击该通知。例如对于一条QQ消息，点击会进入具体的聊天界面。
        /// </summary>
        public void click()
        {
            try
            {
                ContentIntent.Send();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// 删除该通知。该通知将从通知栏中消失。
        /// </summary>
        public void delete()
        {
            try
            {
                DeleteIntent.Send();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public override string ToString()
        {
            return Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat ? $"Notification{{ packageName = { packageName } , title= { title } , text={ text } }} " : base.ToString();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="n"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        internal static Notification Create(Android.App.Notification n, string packageName)
        {
            var notification = new Notification(packageName);
            Clone(n, notification);
            return notification;
        }
        
        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private static void Clone(Android.App.Notification from, Android.App.Notification to)
        {
            to.When = from.When;
            to.Icon = from.Icon;
            to.IconLevel = from.IconLevel;
            to.Number = from.Number;
            to.ContentIntent = from.ContentIntent;
            to.DeleteIntent = from.DeleteIntent;
            to.FullScreenIntent = from.FullScreenIntent;
            to.TickerText = from.TickerText;
            to.TickerView = from.TickerView;
            to.ContentView = from.ContentView;
            to.BigContentView = from.BigContentView;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                to.HeadsUpContentView = from.HeadsUpContentView;
                to.AudioAttributes = from.AudioAttributes;
                to.Color = from.Color;
                to.Visibility = from.Visibility;
                to.Category = from.Category;
                to.PublicVersion = from.PublicVersion;
            }
            to.LargeIcon = from.LargeIcon;
            to.Sound = from.Sound;
            to.AudioStreamType = from.AudioStreamType;
            to.Vibrate = from.Vibrate;
            to.LedARGB = from.LedARGB;
            to.LedOnMS = from.LedOnMS;
            to.LedOffMS = from.LedOffMS;
            to.Defaults = from.Defaults;
            to.Flags = from.Flags;
            to.Priority = from.Priority;
            
            if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat) 
                return;
            
            to.Extras = @from.Extras;
            to.Actions = @from.Actions;
        }
    }
}