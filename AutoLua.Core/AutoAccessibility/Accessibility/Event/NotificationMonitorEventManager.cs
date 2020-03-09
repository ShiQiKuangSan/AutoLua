﻿using System;
using System.Collections.Generic;
using AutoLua.Core.AutoAccessibility.Accessibility.Node;

namespace AutoLua.Core.AutoAccessibility.Accessibility.Event
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
        private readonly IList<INotificationMonitorEvent> _notificationMonitorEvent = new List<INotificationMonitorEvent>();

        /// <summary>
        /// 添加按键监听事件。
        /// </summary>
        /// <param name="event"></param>
        public void Add(INotificationMonitorEvent @event)
        {
            if (@event == null)
                return;

            _notificationMonitorEvent.Add(@event);
        }

        /// <summary>
        /// 移除按键监听事件。
        /// </summary>
        /// <param name="event"></param>
        public void Remove(INotificationMonitorEvent @event)
        {
            if (@event == null)
                return;

            _notificationMonitorEvent.Remove(@event);
        }


        public void OnNotification(Notification notification)
        {
            foreach (var item in _notificationMonitorEvent)
            {
                try
                {
                    item.OnNotification(notification);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}