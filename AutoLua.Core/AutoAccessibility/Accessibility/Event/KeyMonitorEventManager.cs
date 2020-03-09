using System;
using System.Collections.Generic;
using Android.Views;

namespace AutoLua.Core.AutoAccessibility.Accessibility.Event
{
    /// <summary>
    /// 按键监听事件管理器。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class KeyMonitorEventManager
    {
        /// <summary>
        /// 监听按键的缓存
        /// </summary>
        private readonly IList<IKeyMonitorEvent> _keyMonitorEvents = new List<IKeyMonitorEvent>();

        /// <summary>
        /// 添加按键监听事件。
        /// </summary>
        /// <param name="event"></param>
        public void Add(IKeyMonitorEvent @event)
        {
            if (@event == null)
                return;

            _keyMonitorEvents.Add(@event);
        }

        /// <summary>
        /// 移除按键监听事件。
        /// </summary>
        /// <param name="event"></param>
        public void Remove(IKeyMonitorEvent @event)
        {
            if (@event == null)
                return;

            _keyMonitorEvents.Remove(@event);
        }

        /// <summary>
        /// 监听按键
        /// </summary>
        /// <param name="keyCode">按键码</param>
        /// <param name="event">按键事件</param>
        public void OnKeyEvent(Keycode keyCode, KeyEvent @event)
        {
            foreach (var item in _keyMonitorEvents)
            {
                try
                {
                    item.OnKeyEvent(keyCode, @event);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}