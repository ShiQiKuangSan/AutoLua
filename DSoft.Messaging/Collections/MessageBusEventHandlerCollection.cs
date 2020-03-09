using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DSoft.Messaging.Collections
{
    /// <summary>
    /// 消息总线事件处理程序的集合
    /// </summary>
    public class MessageBusEventHandlerCollection : Collection<MessageBusEventHandler>
    {
        #region 方法

        /// <summary>
        /// 事件的处理程序
        /// </summary>
        /// <param name="eventId">事件标识符</param>
        /// <returns></returns>
        internal MessageBusEventHandler[] HandlersForEvent (string eventId)
        {
            var results = from item in Items
                where !string.IsNullOrWhiteSpace (item.EventId)
                where item.EventId.ToLower ().Equals (eventId.ToLower ())
                where item.EventAction != null
                select item;

            var array = results.ToArray ();
            return array;
        }

        /// <summary>
        /// 事件类型的处理程序
        /// </summary>
        /// <returns>The for event.</returns>
        /// <param name="eventType">事件类型.</param>
        internal MessageBusEventHandler[] HandlersForEvent (Type eventType)
        {
            var results = from item in this.Items
                where item is TypedMessageBusEventHandler
                where item.EventAction != null
                select item;

            return results.ToArray()
                .Cast<TypedMessageBusEventHandler>()
                .Where(item => item.EventType != null && item.EventType == eventType)
                .Cast<MessageBusEventHandler>()
                .ToArray();
        }

        /// <summary>
        /// 返回指定Generic MessageBusEvent类型的事件处理程序
        /// </summary>
        /// <returns>The for event.</returns>
        /// <typeparam name="T">第一种类型参数</typeparam>
        internal MessageBusEventHandler[] HandlersForEvent<T> () where T : MessageBusEvent
        {
            return HandlersForEvent (typeof(T));
        }

        #endregion
    }
}