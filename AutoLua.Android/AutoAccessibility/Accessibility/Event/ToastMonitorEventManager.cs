using System;
using System.Collections.Generic;

namespace AutoLua.Droid.AutoAccessibility.Accessibility.Event
{
    /// <summary>
    /// Toast 监听事件管理器
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ToastMonitorEventManager
    {
        /// <summary>
        /// 监听按键的缓存
        /// </summary>
        private readonly IList<IToastMonitorEvent> toastMonitorEvent = new List<IToastMonitorEvent>();

        /// <summary>
        /// 添加按键监听事件。
        /// </summary>
        /// <param name="event"></param>
        public void Add(IToastMonitorEvent @event)
        {
            if (@event == null)
                return;

            toastMonitorEvent.Add(@event);
        }

        /// <summary>
        /// 移除按键监听事件。
        /// </summary>
        /// <param name="event"></param>
        public void Remove(IToastMonitorEvent @event)
        {
            if (@event == null)
                return;

            toastMonitorEvent.Remove(@event);
        }

        public void OnToast(Node.Toast toast)
        {
            foreach (var item in toastMonitorEvent)
            {
                try
                {
                    item.OnToast(toast);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}