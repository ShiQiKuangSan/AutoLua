using AutoLua.Droid.AutoAccessibility.Accessibility.Node;
using System;
using System.Collections.Generic;

namespace AutoLua.Droid.AutoAccessibility.Accessibility.Event
{
    /// <summary>
    /// 通知了监听事件管理器
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class NotificationMonitorEventManager
    {
        /// <summary>
        /// 监听按键的缓存
        /// </summary>
        private readonly IList<INotificationMonitorEvent> notificationMonitorEvent = new List<INotificationMonitorEvent>();

        /// <summary>
        /// 添加按键监听事件。
        /// </summary>
        /// <param name="event"></param>
        public void Add(INotificationMonitorEvent @event)
        {
            if (@event == null)
                return;

            notificationMonitorEvent.Add(@event);
        }

        /// <summary>
        /// 移除按键监听事件。
        /// </summary>
        /// <param name="event"></param>
        public void Remove(INotificationMonitorEvent @event)
        {
            if (@event == null)
                return;

            notificationMonitorEvent.Remove(@event);
        }


        public void OnNotification(Notification notification)
        {
            foreach (var item in notificationMonitorEvent)
            {
                try
                {
                    item.OnNotification(notification);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}