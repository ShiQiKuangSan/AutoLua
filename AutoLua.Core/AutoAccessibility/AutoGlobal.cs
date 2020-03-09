using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Text;
using AutoLua.Core.AutoAccessibility.Accessibility.Event;
using Java.Lang;

namespace AutoLua.Core.AutoAccessibility
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public sealed class AutoGlobal
    {
        internal readonly Context Context;

        /// <summary>
        /// 无障碍服务事件接口
        /// </summary>
        public AccessibilityEventWindos AccessibilityEvent { get; }

        /// <summary>
        /// 无障碍服务事件接口(通知事件)
        /// </summary>
        public AccessibilityEventNotifications  AccessibilityEventNotifications { get; }

        /// <summary>
        /// 按键拦截事件。
        /// </summary>
        public KeyInterceptorEventManager KeyInterceptorEvent { get; }

        /// <summary>
        /// 按键监听事件。
        /// </summary>
        public KeyMonitorEventManager KeyMonitorEvent { get; }

        public NotificationMonitorEventManager NotificationMonitorEvent { get; }

        public ToastMonitorEventManager ToastMonitorEvent { get; }

        /// <summary>
        /// 一个有序的事件集合
        /// </summary>
        internal readonly IList<IAccessibilityEvent> Events;

        private AutoGlobal(Context application)
        {
            Context = application.ApplicationContext;
            Events = new List<IAccessibilityEvent>();
            AccessibilityEvent = new AccessibilityEventWindos(application.PackageManager);
            AccessibilityEventNotifications = new AccessibilityEventNotifications();

            KeyInterceptorEvent = new KeyInterceptorEventManager();
            KeyMonitorEvent = new KeyMonitorEventManager();
            NotificationMonitorEvent = new NotificationMonitorEventManager();
            ToastMonitorEvent = new ToastMonitorEventManager();

            AddEvent();
        }

        /// <summary>
        /// 确保无障碍服务是否启动
        /// </summary>
        public void EnsureAccessibilityServiceEnabled()
        {
            if (AutoAccessibilityService.Instance == null)
                return;

            var errorMsg = string.Empty;

            if (IsAccessibilityServiceEnabled())
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(errorMsg)) 
                return;
            
            GoToAccessibilitySetting();
        }

        /// <summary>
        /// 无障碍服务是否启动。
        /// </summary>
        /// <returns></returns>
        public bool IsAccessibilityServiceEnabled()
        {
            var expectedComponentName = new ComponentName(Context, Class.FromType(typeof(AutoAccessibilityService)));
            var enabledServicesSetting = Android.Provider.Settings.Secure.GetString(Context.ContentResolver, Android.Provider.Settings.Secure.EnabledAccessibilityServices);

            if (enabledServicesSetting == null)
                return false;

            var colonSplitter = new TextUtils.SimpleStringSplitter(':');
            colonSplitter.SetString(enabledServicesSetting);

            while (colonSplitter.HasNext)
            {
                var componentNameString = colonSplitter.Next();
                var enabledService = ComponentName.UnflattenFromString(componentNameString);

                if (enabledService != null && enabledService.ClassName == expectedComponentName.ClassName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 跳转到无障碍服务。
        /// </summary>
        public void GoToAccessibilitySetting()
        {
            Context.StartActivity(new Intent(Android.Provider.Settings.ActionAccessibilitySettings).AddFlags(ActivityFlags.NewTask));
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        private void AddEvent()
        {
            Events.Add(AccessibilityEventNotifications);
            Events.Add(AccessibilityEvent);
        }

        private static AutoGlobal _instance;

        private static readonly object Lock = new object();

        /// <summary>
        /// 自动操作实例
        /// </summary>
        public static AutoGlobal Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new System.Exception("AutoGlobal初始化失败，或未初始化，请继承 AutoApplication后在使用");
                }

                return _instance;
            }
        }

        public static void Init(Application context)
        {
            if (_instance != null) 
                return;
            
            lock (Lock)
            {
                _instance = new AutoGlobal(context);
            }
        }
    }
}