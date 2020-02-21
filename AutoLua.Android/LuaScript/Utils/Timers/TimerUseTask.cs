using System;
using System.Collections.Generic;

namespace AutoLua.Droid.LuaScript.Utils.Timers
{
    /// <summary>
    /// 指定时间执行。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class TimerUseTask : BackgroundTaskUseTimeBase<object>
    {
        private readonly Action<object> _onExecute;
        private readonly IList<object> _tasks = new List<object>();
        private DateTime _nextTime;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="action">执行的函数</param>
        /// <param name="second">下次任务刷新间隔时间（单位：秒）。</param>
        /// <param name="startSecond">多少秒后启动任务。</param>
        public TimerUseTask(Action<object> action, int second, int startSecond = 1) : base(second, startSecond)
        {
            _onExecute = action;
        }

        /// <summary>
        /// 添加定时任务。
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(object task)
        {
            _tasks.Add(task);
        }

        /// <summary>
        /// 获得可处理的任务集合。
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetTasks()
        {
            return _tasks;
        }

        /// <summary>
        /// 执行任务处理。
        /// </summary>
        /// <param name="task"></param>
        protected override void OnExecute(object task)
        {
            _onExecute?.Invoke(task);
        }

        /// <summary>
        /// 设置下次执行任务的时间。
        /// </summary>
        /// <param name="dateTime">下次执行的时间。</param>
        public void SetNextTaskTime(DateTime dateTime)
        {
            _nextTime = dateTime;
        }

        /// <summary>
        /// 获得下一个任务的执行时间。
        /// </summary>
        /// <returns></returns>
        protected override DateTime GetNextTaskTime()
        {
            return _nextTime;
        }
    }
}