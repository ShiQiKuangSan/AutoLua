using System;
using System.Collections.Generic;
using AutoLua.Core.AutoAccessibility.Accessibility.Node;

namespace AutoLua.Core.AutoAccessibility.Accessibility.Event
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
        private readonly IList<IToastMonitorEvent> _toastMonitorEvent = new List<IToastMonitorEvent>();

        /// <summary>
        /// 添加按键监听事件。
        /// </summary>
        /// <param name="event"></param>
        public void Add(IToastMonitorEvent @event)
        {
            if (@event == null)
                return;

            _toastMonitorEvent.Add(@event);
        }

        /// <summary>
        /// 移除按键监听事件。
        /// </summary>
        /// <param name="event"></param>
        public void Remove(IToastMonitorEvent @event)
        {
            if (@event == null)
                return;

            _toastMonitorEvent.Remove(@event);
        }

        public void OnToast(Toast toast)
        {
            foreach (var item in _toastMonitorEvent)
            {
                try
                {
                    item.OnToast(toast);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}