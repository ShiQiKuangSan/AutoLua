using System;

namespace DSoft.Messaging.Extensions
{
    /// <summary>
    /// MessageBus对象扩展
    /// </summary>
    public static class MessageBusExtensions
    {
        /// <summary>
        /// Posts 事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="eventId">事件 Id</param>
        public static void PostEvent (this object sender, string eventId)
        {
            sender.PostEvent (eventId, null);
        }

        /// <summary>
        /// Posts 事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="eventId">事件 Id</param>
        /// <param name="data">附加数据</param>
        public static void PostEvent (this object sender, string eventId, object[] data)
        {
            MessageBus.Default.Post (eventId, sender, data);
        }
    }
}