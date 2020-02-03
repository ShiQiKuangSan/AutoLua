using System.Linq;
using Android.AccessibilityServices;
using Android.Views.Accessibility;
using AutoLua.Droid.AutoAccessibility.Accessibility.Node;
using AutoLua.Droid.Utils;

namespace AutoLua.Droid.AutoAccessibility.Accessibility.Event
{
    /// <summary>
    /// 无障碍服务事件接口(通知事件)
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class AccessibilityEventNotifications : IAccessibilityEvent
    {
        /// <summary>
        /// 无障碍服务发送的消息，进行分类处理，这里是处理的通知。
        /// </summary>
        /// <param name="autoAccessibilityService"></param>
        /// <param name="e"></param>
        public void Event(AccessibilityService autoAccessibilityService, AccessibilityEvent e)
        {
            if (e?.ParcelableData == null)
                return;

            if (string.IsNullOrWhiteSpace(e.PackageName))
                return;

            if (e.ParcelableData is Android.App.Notification notification)
            {
                //通知栏操作
                AutoGlobal.Instance?.NotificationMonitorEvent?.OnNotification(Node.Notification.Create(notification, e.PackageName));
            }
            else
            {
                //当前 autolua 的包名
                var appPackageName = AppUtils.GetAppContext?.PackageName ?? string.Empty;
                if (string.IsNullOrWhiteSpace(appPackageName))
                    return;

                //toast 是 autolua发出的，不监听。
                if (e.PackageName == appPackageName)
                    return;

                if (!e.Text.Any()) 
                    return;
                
                var list = e.Text.Select(x => x.ToString()).ToList();
                AutoGlobal.Instance?.ToastMonitorEvent?.OnToast(new Toast(e.PackageName, list));
            }
        }

    }
}