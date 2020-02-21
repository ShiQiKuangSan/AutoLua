using System;

namespace AutoLua.Droid.LuaScript.Utils.Timers
{
    /// <summary>
    /// 时钟任务。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class TimerTask : BackgroundTaskBase
    {
        private readonly Action _execute;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="action">执行的函数</param>
        /// <param name="interval">时钟周期。(单位：毫秒)</param>
        /// <param name="isRunOne">是否只运行一次</param>
        public TimerTask(Action action, double interval, bool isRunOne = true) : base(interval)
        {
            _execute = action;
            stop = isRunOne;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="action">执行的函数</param>
        /// <param name="executeTime">指定的时间开始执行（只执行一次）。</param>
        /// <param name="isRunOne">是否只运行一次</param>
        public TimerTask(Action action, DateTime executeTime, bool isRunOne = true) : base(executeTime)
        {
            _execute = action;
            stop = isRunOne;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="action">执行的函数</param>
        /// <param name="executeTime">指定的时间开始执行。</param>
        /// <param name="interval">时钟周期（单位：毫秒）。</param>
        /// <param name="isRunOne">是否只运行一次</param>
        public TimerTask(Action action, DateTime executeTime, double interval, bool isRunOne = true) : base(executeTime,
            interval)
        {
            _execute = action;
            stop = isRunOne;
        }

        protected override void OnExecute()
        {
            _execute?.Invoke();
        }
    }
}