using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSoft.Messaging.Collections;

namespace DSoft.Messaging
{
    /// <summary>
    /// 消息总线
    /// </summary>
	public class MessageBus
	{
		#region Fields

		private static volatile MessageBus _default;
		private static readonly object SyncRoot = new object();
		private MessageBusEventHandlerCollection _eventHandlers;

		#endregion

		#region Constructors

		public MessageBus()
		{
		}

		#endregion
		#region Properties

		/// <summary>
		/// 获取默认消息总线
		/// </summary>
		/// <value>默认值</value>
		public static MessageBus Default {
			get
			{
				if (_default != null) 
					return _default;
				
				lock (SyncRoot) 
				{
					if (_default == null)
						_default = new MessageBus ();
				}

				return _default;
			}
		}

		/// <summary>
		/// 获取注册的事件处理程序
		/// </summary>
		/// <value>事件处理程序</value>
		internal MessageBusEventHandlerCollection EventHandlers => _eventHandlers ?? (_eventHandlers = new MessageBusEventHandlerCollection());

		/// <summary>
		/// 获取或设置UI线程的同步上下文
		/// </summary>
		/// <value>同步上下文</value>
		public TaskScheduler SyncContext
		{
			get;
			set;
		}
		
		#endregion

		#region Methods

		#region Registration

		/// <summary>
		/// 注册指定的事件处理程序
		/// </summary>
		/// <param name="eventHandler">事件处理程序</param>
		public void Register (MessageBusEventHandler eventHandler)
		{
			if (eventHandler == null)
				return;

			if (!EventHandlers.Contains (eventHandler))
			{
				EventHandlers.Add (eventHandler);
			}
		}

		/// <summary>
		/// 取消注册事件处理程序
		/// </summary>
		/// <param name="eventHandler">事件处理程序</param>
		public void DeRegister (MessageBusEventHandler eventHandler)
		{
			if (EventHandlers.Contains (eventHandler))
			{
				EventHandlers.Remove (eventHandler);
			}
		}

		/// <summary>
		/// 清除指定事件ID的处理程序
		/// </summary>
		/// <param name="eventId">事件ID</param>
		public void Clear (string eventId)
		{
			foreach (var item in EventHandlers.HandlersForEvent(eventId))
			{
				EventHandlers.Remove (item);
			}
		}

		#endregion

		#region Generic Methods

		/// <summary>
		/// 注册一种MessageBusEvent类型
		/// </summary>
		/// <typeparam name="T">一种类型参数</typeparam>
		public void Register<T> (Action<object,MessageBusEvent> action) where T : MessageBusEvent, new()
		{
			var aType = typeof(T);

			var typeHandler = new TypedMessageBusEventHandler () {
				EventType = aType,
				EventAction = action,
			};

			EventHandlers.Add (typeHandler);
		}

		/// <summary>
		/// 注销通用消息总线类型的事件操作
		/// </summary>
		/// <param name="action">Action</param>
		/// <typeparam name="T">一种类型参数</typeparam>
		public void DeRegister<T> (Action<object,MessageBusEvent> action) where T : MessageBusEvent
		{
			var results = new List<MessageBusEventHandler> (EventHandlers.HandlersForEvent<T> ());

			foreach (var item in results.Where(item => item.EventAction == action))
			{
				EventHandlers.Remove (item);
			}
		}

		#endregion

		#region Posting

		private static void Execute (Action<object,MessageBusEvent> action, object sender, MessageBusEvent evnt)
		{
            action(sender, evnt);
		}

		/// <summary>
		/// Post 事件
		/// </summary>
		/// <param name="event">事件对象</param>
		public void Post (MessageBusEvent @event)
		{
			if (!(@event is CoreMessageBusEvent))
			{
				foreach (var item in EventHandlers.HandlersForEvent(@event.GetType()))
				{
					if (item.EventAction != null)
					{
						Execute (item.EventAction, @event.Sender, @event);
					}
				}
			}

			//find all the registered handlers
			foreach (var item in EventHandlers.HandlersForEvent(@event.EventId))
			{
				if (item.EventAction != null)
				{
					Execute (item.EventAction, @event.Sender, @event);
				}
			}

		}

		/// <summary>
		/// Post 事件
		/// </summary>
		/// <param name="eventId">事件Id</param>
		/// <param name="sender">事件发送者</param>
		/// <param name="data">随事件传递的数据对象</param>
		public void Post (string eventId, object sender = null, params object[] data)
		{
			var aEvent = new CoreMessageBusEvent (eventId) {
				Sender = sender,
				Data = data,
			};

			Post (aEvent);
		}

		#endregion

		#endregion

		#region Static Methods

		/// <summary>
		/// 将指定的事件发布到默认MessageBus
		/// </summary>
		/// <param name="event">Event.</param>
		public static void PostEvent (MessageBusEvent @event)
		{
			Default.Post (@event);
		}

		/// <summary>
		/// 将事件发布到默认MessageBus
		/// </summary>
		/// <param name="eventId">事件ID</param>
		public static void PostEvent (string eventId)
		{
			Default.Post (eventId);
		}

		/// <summary>
		/// 将指定的EventId和Sender发送到默认MessageBus
		/// </summary>
		/// <param name="eventId">事件ID</param>
		/// <param name="sender">事件发送者</param>
		public static void PostEvent (string eventId, object sender)
		{
			Default.Post (eventId, sender);
		}

		/// <summary>
		/// 将指定的EventId，发件人和数据发布到默认MessageBus
		/// </summary>
		/// <param name="eventId">事件ID</param>
		/// <param name="sender">事件发送者</param>
		/// <param name="data">随事件传递的数据对象</param>
		public static void PostEvent (string eventId, object sender, object[] data)
		{
			Default.Post (eventId, sender, data);
		}

		/// <summary>
		/// 在UI线程上执行操作
		/// </summary>
		/// <param name="command">命令</param>
		public void RunOnUiThread(Action command)
		{
            if (SyncContext == null)
                throw new ArgumentNullException ("SyncContext");

            //update the UI
			Task.Factory.StartNew(() =>
			{
				command?.Invoke();
			}, CancellationToken.None, TaskCreationOptions.None, SyncContext);
		}
		#endregion
	}
}