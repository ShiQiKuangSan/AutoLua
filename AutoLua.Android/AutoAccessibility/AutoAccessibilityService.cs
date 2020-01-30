using System;
using System.Collections.Generic;
using System.Linq;
using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Views.Accessibility;
using Android.Widget;
using Java.Util.Concurrent.Locks;

namespace AutoLua.Droid.AutoAccessibility
{
    [Service(Name = "AutoLua.Droid.AutoAccessibility.AutoAccessibilityService", Label = "AutoLua", Enabled = true,
    Permission = "android.permission.BIND_ACCESSIBILITY_SERVICE")]
    [IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
    [MetaData("android.accessibilityservice", Resource = "@xml/accessible_service_config")]
    public sealed class AutoAccessibilityService : AccessibilityService
    {
        public static AutoAccessibilityService Instance = null;

        /// <summary>
        ///
        /// </summary>
        private ICondition _enabled;

        /// <summary>
        /// 锁
        /// </summary>
        private readonly ReentrantLock _lock = new ReentrantLock();

        /// <summary>
        /// 只检测该事件返回的包。
        /// </summary>
        public Func<IList<string>> RunPackageNames = null;

        protected override void OnServiceConnected()
        {
            Instance = this;
            _enabled = _lock.NewCondition();
            base.OnServiceConnected();
            _lock.Lock();
            _enabled.SignalAll();
            _lock.Unlock();

            var autoGlobal = AutoGlobal.Instance;

            if (autoGlobal != null && autoGlobal.Context != null)
            {
                Toast.MakeText(autoGlobal.Context, "服务启动成功", ToastLength.Long).Show();
            }
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

            var status = RunPackageNames?.Invoke().Any(x => x == e.PackageName) ?? false;

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
            return AutoGlobal.Instance?.KeyInterceptorEvent?.OnInterceptKeyEvent(e) ?? false; ;
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