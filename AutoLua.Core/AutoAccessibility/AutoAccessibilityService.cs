using System;
using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Views.Accessibility;
using Android.Widget;
using DSoft.Messaging;
using Java.Util.Concurrent.Locks;

namespace AutoLua.Core.AutoAccessibility
{
    [Register("AutoLua.Core.AutoAccessibility.AutoAccessibilityService")]
    [Service(Name = "AutoLua.Core.AutoAccessibility.AutoAccessibilityService", Label = "AutoLua", Enabled = true,
        Permission = "android.permission.BIND_ACCESSIBILITY_SERVICE")]
    [IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
    [MetaData("android.accessibilityservice", Resource = "@xml/accessible_service_config")]
    public sealed class AutoAccessibilityService : AccessibilityService
    {
        public static AutoAccessibilityService Instance;

        public static readonly Func<Intent> StartActivityIntent = null;

        protected override void OnServiceConnected()
        {
            Instance = this;
            base.OnServiceConnected();
            var autoGlobal = AutoGlobal.Instance;

            if (autoGlobal?.Context != null)
            {
                Toast.MakeText(autoGlobal.Context, "服务启动成功", ToastLength.Long).Show();
            }

            //执行启动主界面的事件
            MessageBus.Default.Post("StartActivityMessage", this, "MainActivity");
        }

        public override void OnAccessibilityEvent(AccessibilityEvent e)
        {
            Instance = this;

            if (e == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(e.PackageName))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(e.ClassName))
            {
                return;
            }

            if (AutoGlobal.Instance?.Events == null)
                return;

            foreach (var item in AutoGlobal.Instance?.Events)
            {
                item.Event(this, e);
            }
        }

        public override AccessibilityNodeInfo RootInActiveWindow
        {
            get
            {
                try
                {
                    return base.RootInActiveWindow;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 按键监听
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override bool OnKeyEvent(KeyEvent e)
        {
            AutoGlobal.Instance?.KeyMonitorEvent?.OnKeyEvent(e.KeyCode, e);
            return AutoGlobal.Instance?.KeyInterceptorEvent?.OnInterceptKeyEvent(e) ?? false;
            ;
        }

        public override void OnInterrupt()
        {
        }

        public override void OnDestroy()
        {
            Instance = null;
            base.OnDestroy();
        }
    }
}