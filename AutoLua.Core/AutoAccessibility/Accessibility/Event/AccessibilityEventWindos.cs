using Android.AccessibilityServices;
using Android.Content;
using Android.Content.PM;
using Android.Views.Accessibility;
using System.Runtime.InteropServices;

namespace AutoLua.Core.AutoAccessibility.Accessibility.Event
{
    /// <summary>
    /// 无障碍服务事件接口
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public sealed class AccessibilityEventWindos : IAccessibilityEvent
    {
        /// <summary>
        /// 最后的包名。
        /// </summary>
        public string LatestPackage { get; private set; } = "";

        /// <summary>
        /// 最后的活动名称。
        /// </summary>
        public string LatestActivity { get; private set; } = "";

        /// <summary>
        /// 包管理器
        /// </summary>
        private readonly PackageManager _packageManager;

        /// <summary>
        /// 初始化事件
        /// </summary>
        /// <param name="packageManager"></param>
        public AccessibilityEventWindos(PackageManager packageManager)
        {
            _packageManager = packageManager;
        }

        /// <summary>
        /// 接收事件
        /// </summary>
        /// <param name="autoAccessibilityService">无障碍服务</param>
        /// <param name="e">无障碍服务事件</param>
        public void Event(AccessibilityService autoAccessibilityService, AccessibilityEvent e)
        {
            /**
             * TYPE_WINDOW_STATE_CHANGED
             * 打开popupwindow,菜单，对话框时候会触发
             *
             * TYPE_WINDOWS_CHANGED
             * 窗口的变化
             *
             * TYPE_WINDOW_CONTENT_CHANGED
             * 更加精确的代表了基于当前event.source中的子view的内容变化
             *
             * TYPE_NOTIFICATION_STATE_CHANGED
             * 基本窗口view的变化都可以使用这个type来监听
             */
            SetLatestComponent(e.PackageName, e.ClassName);
        }

        /// <summary>
        /// 设置最后一次类名和包名
        /// </summary>
        /// <param name="latestPackage"></param>
        /// <param name="latestClass"></param>
        private void SetLatestComponent(string latestPackage, string latestClass)
        {
            if (string.IsNullOrWhiteSpace(latestPackage) || string.IsNullOrWhiteSpace(latestClass))
            {
                return;
            }

            if (latestClass.StartsWith("android.view.") || latestClass.StartsWith("android.widget."))
            {
                return;
            }

            try
            {
                if (IsPackageExists(latestPackage))
                {
                    LatestPackage = latestPackage;
                    var componentName = new ComponentName(latestPackage, latestClass);
                    LatestActivity = _packageManager?.GetActivityInfo(componentName, 0)?.Name ?? string.Empty;
                }
            }
            catch (PackageManager.NameNotFoundException e)
            {
                e.PrintStackTrace();
            }
        }

        private  bool IsPackageExists(string packageName)
        {
            try
            {
                _packageManager.GetPackageInfo(packageName, 0);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}