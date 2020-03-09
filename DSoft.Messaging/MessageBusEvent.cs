using System;

namespace DSoft.Messaging
{
    /// <summary>
    /// 消息总线事件类
    /// </summary>
    public abstract class MessageBusEvent
    {
        #region 属性

        /// <summary>
        /// 获取或设置事件标识符
        /// </summary>
        /// <value>事件标识符</value>
        public abstract string EventId { get; }

        /// <summary>
        /// 事件发送者
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        /// 随事件传递的数据
        /// </summary>
        public object[] Data { get; set; }

        #endregion

        #region 构造器

        /// <summary>
        /// 初始化类的新实例 <see cref="DSoft.Messaging.MessageBusEvent"/>
        /// </summary>
        public MessageBusEvent()
        {
        }

        /// <summary>
        /// 初始化类的新实例 <see cref="DSoft.Messaging.MessageBusEvent"/>
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="data">随事件传递的数据</param>
        public MessageBusEvent(object sender, object[] data)
        {
            Sender = sender;
            Data = data;
        }

        #endregion
    }

    /// <summary>
    /// 标准MessageBusEvent类
    /// </summary>
    public sealed class CoreMessageBusEvent : MessageBusEvent
    {
        #region 字段

        private string _eventId;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置事件标识符。 如果未设置，将生成一个基于Guid的新ID
        /// </summary>
        /// <value>事件标识符</value>
        public override string EventId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_eventId))
                {
                    _eventId = Guid.NewGuid().ToString();
                }

                return _eventId;
            }
        }

        #endregion
        
        #region Constructors

        /// <summary>
        /// 初始化类的新实例 <see cref="DSoft.Messaging.CoreMessageBusEvent"/>
        /// </summary>
        public CoreMessageBusEvent ()
        {
			
        }

        /// <summary>
        /// 初始化类的新实例 <see cref="DSoft.Messaging.CoreMessageBusEvent"/>
        /// </summary>
        /// <param name="eventId">Event Identifier.</param>
        public CoreMessageBusEvent (string eventId) 
            : this ()
        {
            _eventId = eventId;
        }

        /// <summary>
        /// 初始化类的新实例 <see cref="DSoft.Messaging.CoreMessageBusEvent"/>
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="data">随事件传递的数据</param>
        public CoreMessageBusEvent (object sender, string eventId, object[] data) 
            : base (sender, data)
        {
            _eventId = eventId;
        }

        #endregion
    }
}