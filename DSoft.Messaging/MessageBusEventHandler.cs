using System;

namespace DSoft.Messaging
{
    /// <summary>
    /// 消息总线事件处理程序
    /// </summary>
    public class MessageBusEventHandler
    {
        #region 属性

        /// <summary>
        /// 事件 ID
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// 对事件执行的动作
        /// </summary>
        public Action<object, MessageBusEvent> EventAction { get; set; }

        #endregion

        #region 构造器

        /// <summary>
        /// 初始化类的新实例 <see cref="DSoft.Messaging.MessageBusEventHandler"/>
        /// </summary>
        public MessageBusEventHandler()
        {
            EventId = string.Empty;
        }
        
        /// <summary>
        /// 初始化类的新实例 <see cref="DSoft.Messaging.MessageBusEventHandler"/>
        /// </summary>
        /// <param name="eventId">事件 ID</param>
        /// <param name="action">对事件执行的动作</param>
        public MessageBusEventHandler (string eventId, Action<object, MessageBusEvent> action)
        {
            EventId = eventId;
            EventAction = action;
        }

        #endregion
    }

    /// <summary>
    /// 键入的消息总线事件处理程序
    /// </summary>
    internal class TypedMessageBusEventHandler : MessageBusEventHandler
    {
        #region 属性

        /// <summary>
        /// 获取或设置事件的类型
        /// </summary>
        /// <value>事件的类型</value>
        internal Type EventType { get; set; }

        #endregion

        #region 构造器

        /// <summary>
        /// 初始化类的新实例 <see cref="DSoft.Messaging.TypedMessageBusEventHandler"/>
        /// </summary>
        internal TypedMessageBusEventHandler()
        {
        }

        #endregion
    }
}