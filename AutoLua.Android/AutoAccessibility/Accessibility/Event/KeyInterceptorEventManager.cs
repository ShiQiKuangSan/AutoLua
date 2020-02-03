using System;
using System.Collections.Generic;
using Android.Views;

namespace AutoLua.Droid.AutoAccessibility.Accessibility.Event
{
    /// <summary>
    /// 按键拦截器，主要是进行按键的屏蔽监听，管理器
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class KeyInterceptorEventManager
    {
        /// <summary>
        /// 拦截的事件缓存。
        /// </summary>
        private readonly IList<IKeyInterceptorEvent> _keyInterceptors = new List<IKeyInterceptorEvent>();

        /// <summary>
        /// 添加拦截事件。
        /// </summary>
        /// <param name="event">拦截事件</param>
        public void Add(IKeyInterceptorEvent @event)
        {
            if (@event == null)
                return;

            _keyInterceptors.Add(@event);
        }

        /// <summary>
        /// 移除按键拦截事件。
        /// </summary>
        /// <param name="event">拦截事件</param>
        public void Remove(IKeyInterceptorEvent @event)
        {
            if (@event == null)
                return;

            _keyInterceptors.Remove(@event);
        }

        /// <summary>
        /// 拦截按键事件。
        /// </summary>
        /// <param name="event">事件</param>
        /// <returns>是否拦截成功。</returns>
        public bool OnInterceptKeyEvent(KeyEvent @event)
        {
            foreach (var item in _keyInterceptors)
            {
                try
                {
                    if (item.OnInterceptKeyEvent(@event))
                        return true;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return false;
        }
    }
}