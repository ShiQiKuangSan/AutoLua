using System;
using System.Collections.Generic;
using Android.AccessibilityServices;
using Android.Views;
using AutoLua.Droid.AutoAccessibility;
using AutoLua.Droid.AutoAccessibility.Accessibility.Event;

namespace AutoLua.Droid.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class Events
    {
        /// <summary>
        /// 是否启动了监听
        /// </summary>
        private bool mListeningKey = false;

        private readonly IDictionary<string, Action> events;

        public Events()
        {
            events = new Dictionary<string, Action>();
        }

        /// <summary>
        /// 启用按键监听，例如音量键、Home键。按键监听使用无障碍服务实现，如果无障碍服务未启用会抛出异常并提示开启。
        /// </summary>
        public void observeKey()
        {
            if (mListeningKey)
                return;

            var service = GetAccessibilityService();
            if (service.ServiceInfo.Flags == AccessibilityServiceFlags.RequestFilterKeyEvents)
                throw new Exception("按键监听未启用，请在软件设置中开启");

            mListeningKey = true;
            AutoGlobal.Instance.KeyMonitorEvent.Add(new EventsApi(this));
        }

        /// <summary>
        /// 按键按下。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Events onKeyDown(string name, Action action)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"onKeyDown : {name} 是空的");
            }

            name = name.ToLower();

            if (events.ContainsKey(name))
                return this;

            events.Add(name, action);

            return this;
        }

        /// <summary>
        /// 获取无障碍服务。
        /// </summary>
        /// <returns></returns>
        private AutoAccessibilityService GetAccessibilityService()
        {
            AutoGlobal.Instance.EnsureAccessibilityServiceEnabled();

            var service = AutoAccessibilityService.Instance;

            if (service == null)
                throw new Exception("AccessibilityService = null");

            return service;
        }


        /// <summary>
        /// 按键监听事件接收器，隔离lua api的使用
        /// </summary>
        private class EventsApi : IKeyMonitorEvent
        {
            private readonly Events events;

            public EventsApi(Events events)
            {
                this.events = events;
            }

            /// <summary>
            /// 监听按键
            /// </summary>
            /// <param name="keyCode">按键码</param>
            /// <param name="event">按键事件</param>
            public void OnKeyEvent(Keycode keyCode, KeyEvent @event)
            {
                var code = keyCode.ToString();

                if (string.IsNullOrWhiteSpace(code))
                    return;

                code = code.ToLower();

                if (events.events.ContainsKey(code))
                    events.events[code]?.Invoke();
            }
        }
    }
}