﻿using Android.Views;

namespace AutoLua.Core.AutoAccessibility.Accessibility.Event
{
    /// <summary>
    /// 按键拦截器，主要是进行按键的屏蔽监听
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public interface IKeyInterceptorEvent
    {
        /// <summary>
        /// 拦截按键事件。
        /// </summary>
        /// <param name="event">事件。</param>
        /// <returns>是否拦截成功。</returns>
        public bool OnInterceptKeyEvent(KeyEvent @event);
    }
}