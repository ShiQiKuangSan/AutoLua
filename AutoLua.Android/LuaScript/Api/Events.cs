using System;
using System.Collections.Generic;
using Android.AccessibilityServices;
using Android.Views;
using AutoLua.Droid.AutoAccessibility;
using AutoLua.Droid.AutoAccessibility.Accessibility.Event;
using AutoLua.Droid.AutoAccessibility.Accessibility.Node;

namespace AutoLua.Droid.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class Events
    {
        /// <summary>
        /// 是否启动了监听
        /// </summary>
        private bool IsKeyListening = false;

        /// <summary>
        /// 是否启动按键拦截。
        /// </summary>
        private bool IsKeyIntercepts = false;

        /// <summary>
        /// 是否监听Toast
        /// </summary>
        private bool IsToastListening = false;

        /// <summary>
        /// 是否监听通知栏
        /// </summary>
        private bool IsNotificationListening = false;

        /// <summary>
        /// 按键监听事件集合
        /// </summary>
        private readonly IDictionary<string, Action> events;

        /// <summary>
        /// 触发一次的事件集合
        /// </summary>
        private readonly IDictionary<string, Action> eventsOnce;

        /// <summary>
        /// 拦截按键的key集合。
        /// </summary>
        private readonly HashSet<string> interceptedKeys;

        private const string OnKeyDownKey = "keyDown_";
        private const string OnKeyUpKey = "keyUp_";

        private Action<Toast> eventToasts;
        private Action<Notification> eventNotifications;

        public Events()
        {
            events = new Dictionary<string, Action>();
            eventsOnce = new Dictionary<string, Action>();
            interceptedKeys = new HashSet<string>();

            AutoGlobal.Instance?.KeyInterceptorEvent?.Add(new EventsInterceptorApi(this));
        }

        /// <summary>
        /// 启用按键监听，例如音量键、Home键。按键监听使用无障碍服务实现，如果无障碍服务未启用会抛出异常并提示开启。
        /// </summary>
        public void observeKey()
        {
            if (IsKeyListening)
                return;

            var service = GetAccessibilityService();
            if (service.ServiceInfo.Flags == AccessibilityServiceFlags.RequestFilterKeyEvents)
                throw new Exception("按键监听未启用，请在软件设置中开启");

            IsKeyListening = true;
            AutoGlobal.Instance?.KeyMonitorEvent?.Add(new EventsApi(this));
        }

        /// <summary>
        /// 开启Toast监听。
        /// Toast监听依赖于无障碍服务，因此此函数会确保无障碍服务运行。
        /// </summary>
        public void observeToast()
        {
            if (IsToastListening)
                return;

            GetAccessibilityService();
            IsToastListening = true;
            AutoGlobal.Instance?.ToastMonitorEvent?.Add(new EventsToastApi(this));
        }

        /// <summary>
        /// 开启通知监听。例如QQ消息、微信消息、推送等通知。
        /// 通知监听依赖于通知服务，如果通知服务没有运行，会抛出异常并跳转到通知权限开启界面。（有时即使通知权限已经开启通知服务也没有运行，这时需要关闭权限再重新开启一次）
        /// </summary>
        public void observeNotification()
        {
            if (IsNotificationListening)
                return;

            GetAccessibilityService();
            IsNotificationListening = true;
            AutoGlobal.Instance?.NotificationMonitorEvent?.Add(new EventsNotificationApi(this));
        }

        /// <summary>
        /// 按键按下。
        /// </summary>
        /// <param name="name">要监听的按键名称</param>
        /// <param name="action">按键监听器。</param>
        /// <returns></returns>
        public Events onKeyDown(string name, Action action)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"onKeyDown : {name} 是空的");
            }

            if (action == null)
                throw new Exception($"onKeyDown :  回调函数不存在");

            name = name.ToLower();

            if (events.ContainsKey($"{OnKeyDownKey}{name}"))
                return this;

            events.Add($"{OnKeyDownKey}{name}", action);

            return this;
        }

        /// <summary>
        /// 按键弹起。
        /// </summary>
        /// <param name="name">要监听的按键名称</param>
        /// <param name="action">按键监听器。</param>
        /// <returns></returns>
        public Events onKeyUp(string name, Action action)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"onKeyUp : {name} 是空的");
            }

            if (action == null)
                throw new Exception($"onKeyUp :  回调函数不存在");

            name = name.ToLower();

            if (events.ContainsKey($"{OnKeyUpKey}{name}"))
                return this;

            events.Add($"{OnKeyUpKey}{name}", action);

            return this;
        }


        /// <summary>
        /// 注册一个按键监听函数，当有keyName对应的按键被按下时会调用该函数，之后会注销该按键监听器。
        /// 也就是listener只有在onceKeyDown调用后的第一次按键事件被调用一次。
        /// </summary>
        /// <param name="name">要监听的按键名称</param>
        /// <param name="action">按键监听器。</param>
        /// <returns></returns>
        public Events onceKeyDown(string name, Action action)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"onceKeyDown : {name} 是空的");
            }

            if (action == null)
                throw new Exception($"onceKeyDown :  回调函数不存在");

            name = name.ToLower();

            if (eventsOnce.ContainsKey($"{OnKeyDownKey}{name}"))
                return this;

            //调用一次回调后，删除当前按键监听
            var OnceKeyDown = new Action(() =>
            {
                if (eventsOnce.ContainsKey($"{OnKeyDownKey}{name}"))
                    eventsOnce.Remove($"{OnKeyDownKey}{name}");

                action.Invoke();
            });

            eventsOnce.Add($"{OnKeyDownKey}{name}", OnceKeyDown);

            return this;
        }

        /// <summary>
        /// 注册一个按键监听函数，当有keyName对应的按键弹起时会调用该函数，之后会注销该按键监听器。
        /// 也就是listener只有在onceKeyUp调用后的第一次按键事件被调用一次。
        /// </summary>
        /// <param name="name">要监听的按键名称</param>
        /// <param name="action">按键监听器。</param>
        /// <returns></returns>
        public Events onceKeyUp(string name, Action action)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"onceKeyUp : {name} 是空的");
            }

            if (action == null)
                throw new Exception($"onceKeyUp :  回调函数不存在");

            name = name.ToLower();

            if (eventsOnce.ContainsKey($"{OnKeyUpKey}{name}"))
                return this;

            //调用一次回调后，删除当前按键监听
            var OnceKeyDown = new Action(() =>
            {
                if (eventsOnce.ContainsKey($"{OnKeyUpKey}{name}"))
                    eventsOnce.Remove($"{OnKeyUpKey}{name}");

                action.Invoke();
            });

            eventsOnce.Add($"{OnKeyUpKey}{name}", OnceKeyDown);

            return this;
        }

        /// <summary>
        /// 删除该按键的KeyDown(按下)事件的所有监听。
        /// </summary>
        /// <param name="keyName">按键名称</param>
        /// <returns></returns>
        public Events removeAllKeyDown(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName))
                throw new Exception($"removeAllKeyDown : {keyName} 是空的");

            if (events.ContainsKey($"{OnKeyDownKey}{keyName}"))
                events.Remove($"{OnKeyDownKey}{keyName}");

            if (eventsOnce.ContainsKey($"{OnKeyDownKey}{keyName}"))
                eventsOnce.Remove($"{OnKeyDownKey}{keyName}");

            return this;
        }

        /// <summary>
        /// 删除该按键的KeyUp(弹起)事件的所有监听。
        /// </summary>
        /// <param name="keyName">按键名称</param>
        /// <returns></returns>
        public Events removeAllKeyUp(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName))
                throw new Exception($"removeAllKeyUp : {keyName} 是空的");

            if (events.ContainsKey($"{OnKeyUpKey}{keyName}"))
                events.Remove($"{OnKeyUpKey}{keyName}");

            if (eventsOnce.ContainsKey($"{OnKeyUpKey}{keyName}"))
                eventsOnce.Remove($"{OnKeyUpKey}{keyName}");

            return this;
        }

        /// <summary>
        /// 设置按键屏蔽是否启用。所谓按键屏蔽指的是，屏蔽原有按键的功能，例如使得音量键不再能调节音量，但此时仍然能通过按键事件监听按键。
        /// </summary>
        /// <param name="enabled">是否屏蔽</param>
        public void setKeyInterceptionEnabled(bool enabled = false)
        {
            IsKeyIntercepts = enabled;
        }

        /// <summary>
        /// 设置按键屏蔽是否启用。所谓按键屏蔽指的是，屏蔽原有按键的功能，例如使得音量键不再能调节音量，但此时仍然能通过按键事件监听按键。
        /// </summary>
        /// <param name="keyName">要屏蔽的按键</param>
        /// <param name="enabled">是否屏蔽</param>
        public void setKeyInterceptionEnabled(string keyName, bool enabled = false)
        {
            if (string.IsNullOrWhiteSpace(keyName))
                return;

            if (enabled)
            {
                interceptedKeys.Add(keyName);
            }
            else
            {
                interceptedKeys.Remove(keyName);
            }
        }


        /// <summary>
        /// 当有应用发出toast(气泡消息)时会触发该事件。但AutoLua软件本身的toast除外。
        /// </summary>
        /// <param name="action"></param>
        public void onToast(Action<Toast> action)
        {
            eventToasts = action ?? throw new Exception($"onToast : 回调函数是空的");
        }

        /// <summary>
        /// 开启通知监听。例如QQ消息、微信消息、推送等通知。
        /// 通知监听依赖于通知服务，如果通知服务没有运行，会抛出异常并跳转到通知权限开启界面。（有时即使通知权限已经开启通知服务也没有运行，这时需要关闭权限再重新开启一次）
        /// </summary>
        /// <param name="action"></param>
        public void onNotification(Action<Notification> action)
        {
            eventNotifications = action ?? throw new Exception($"onNotification : 回调函数是空的");
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

                switch (@event.Action)
                {
                    case KeyEventActions.Down:
                        //按键按下事件
                        if (events.events.ContainsKey($"{OnKeyDownKey}{code}"))
                            events.events[$"{OnKeyDownKey}{code}"]?.Invoke();

                        //一次性的按键按下事件
                        if (events.eventsOnce.ContainsKey($"{OnKeyDownKey}{code}"))
                            events.eventsOnce[$"{OnKeyDownKey}{code}"]?.Invoke();
                        break;
                    case KeyEventActions.Up:
                        //按键弹起事件
                        if (events.events.ContainsKey($"{OnKeyUpKey}{code}"))
                            events.events[$"{OnKeyUpKey}{code}"]?.Invoke();

                        //一次性的按键弹起事件
                        if (events.eventsOnce.ContainsKey($"{OnKeyUpKey}{code}"))
                            events.eventsOnce[$"{OnKeyUpKey}{code}"]?.Invoke();
                        break;
                    default:
                        //其他事件
                        break;
                }
            }
        }

        /// <summary>
        /// 按键拦截事件接收器，隔离lua api的使用
        /// </summary>
        private class EventsInterceptorApi : IKeyInterceptorEvent
        {
            private readonly Events events;

            public EventsInterceptorApi(Events events)
            {
                this.events = events;
            }

            /// <summary>
            /// 拦截按键事件。
            /// </summary>
            /// <param name="key">按键。</param>
            /// <returns>是否拦截成功。</returns>
            public bool OnInterceptKeyEvent(KeyEvent @event)
            {
                //启动了拦截，会使系统的音量、Home、返回等键不再具有调节音量、回到主页、返回的作用，但此时仍然能通过按键事件监听按键。
                if (events.IsKeyIntercepts)
                    return true;

                var code = @event.KeyCode.ToString();

                return events.interceptedKeys.Contains(code);
            }
        }

        /// <summary>
        /// toast 监听的回调事件api
        /// </summary>
        private class EventsToastApi : IToastMonitorEvent
        {
            private readonly Events events;

            public EventsToastApi(Events events)
            {
                this.events = events;
            }

            /// <summary>
            /// toast 的回调。
            /// </summary>
            /// <param name="toast">回调消息。</param>
            public void OnToast(Toast toast)
            {
                if (!events.IsToastListening)
                    return;

                events?.eventToasts?.Invoke(toast);
            }
        }

        /// <summary>
        /// 通知栏 监听的回调事件api
        /// </summary>
        private class EventsNotificationApi : INotificationMonitorEvent
        {
            private readonly Events events;

            public EventsNotificationApi(Events events)
            {
                this.events = events;
            }

            public void OnNotification(Notification notification)
            {
                if (!events.IsNotificationListening)
                    return;

                events?.eventNotifications?.Invoke(notification);
            }
        }
    }
}