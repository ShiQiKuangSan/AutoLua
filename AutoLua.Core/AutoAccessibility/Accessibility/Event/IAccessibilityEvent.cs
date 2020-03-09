using Android.AccessibilityServices;
using Android.Views.Accessibility;

namespace AutoLua.Core.AutoAccessibility.Accessibility.Event
{
    /// <summary>
    /// 无障碍服务事件接口
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public interface IAccessibilityEvent
    {
        /// <summary>
        /// 接收事件
        /// </summary>
        /// <param name="autoAccessibilityService">无障碍服务</param>
        /// <param name="e">无障碍服务事件</param>
        void Event(AccessibilityService autoAccessibilityService, AccessibilityEvent e);
    }
}