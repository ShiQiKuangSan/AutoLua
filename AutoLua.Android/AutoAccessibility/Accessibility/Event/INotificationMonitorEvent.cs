namespace AutoLua.Droid.AutoAccessibility.Accessibility.Event
{
    /// <summary>
    /// 通知了监听事件
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public interface INotificationMonitorEvent
    {
        /// <summary>
        /// 监听通知栏。
        /// </summary>
        /// <param name="notification">通知栏</param>
        public void OnNotification(Node.Notification notification);
    }
}