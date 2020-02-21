using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using AutoLua.Droid.Expansions;

namespace AutoLua.Droid.LuaScript.Utils.Timers
{
    /// <summary>
    /// 后台任务。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public abstract class BackgroundTaskBase : IBackgroundTask
    {
        #region 字段

        /// <summary>
        /// 是否停止。
        /// </summary>
        protected bool stop = false;

        /// <summary>
        /// 是否是指定时间执行。
        /// </summary>
        private readonly bool _appointTime = false;

        /// <summary>
        /// 指定时间执行后，每次间隔执行时间。
        /// </summary>
        private readonly double _appointInterval = 0;

        #endregion

        #region 初始化

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="interval">时钟周期。(单位：毫秒)</param>
        protected BackgroundTaskBase(double interval)
        {
            if (interval <= 0)
                throw new ArgumentOutOfRangeException(nameof(interval), "时钟周期数必须大于 0。");

            Id = Guid.NewGuid();

            Timer = new Timer {Interval = interval};

            Timer.Elapsed += Elapsed_Timer;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="executeTime">指定的时间开始执行（只执行一次）。</param>
        protected BackgroundTaskBase(DateTime executeTime)
        {
            if (executeTime <= DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(executeTime), "执行时间不可以是过去的时间");

            Id = Guid.NewGuid();

            Timer = new Timer {Interval = GetDateToMilliseconds(executeTime)};
            Timer.Elapsed += Elapsed_Timer;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="executeTime">指定的时间开始执行。</param>
        /// <param name="interval">时钟周期（单位：毫秒）。</param>
        protected BackgroundTaskBase(DateTime executeTime, double interval)
        {
            if (executeTime <= DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(executeTime), "执行时间不可以是过去的时间");

            if (interval <= 0)
                throw new ArgumentOutOfRangeException(nameof(interval), "时钟周期数必须大于 0。");

            Id = Guid.NewGuid();

            Timer = new Timer {Interval = GetDateToMilliseconds(executeTime)};
            Timer.Elapsed += Elapsed_Timer;

            _appointTime = true;
            _appointInterval = interval;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 标识。
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 是否启用。
        /// </summary>
        public bool Enabled => Timer.Enabled;

        /// <summary>
        /// 时钟。
        /// </summary>
        protected Timer Timer { get; }

        #endregion

        #region 启动

        /// <summary>
        /// 开始时钟。
        /// </summary>
        public void Start()
        {
            if (!Timer.Enabled)
            {
                Timer.Start();
            }
        }

        #endregion

        #region 终止

        /// <summary>
        /// 终止时钟。
        /// </summary>
        public void Stop()
        {
            if (Timer.Enabled)
            {
                Timer.Stop();
            }
        }

        #endregion

        #region 时钟周期事件

        /// <summary>
        /// 定时触发的事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Elapsed_Timer(object sender, ElapsedEventArgs e)
        {
            try
            {
                lock (this)
                {
                    OnExecute();

                    if (stop)
                        Stop();

                    if (_appointTime)
                    {
                        SetInterval(_appointInterval);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// 事件执行方法。
        /// </summary>
        protected abstract void OnExecute();

        #endregion

        #region 设置时间

        /// <summary>
        /// 将秒转换成毫秒。
        /// </summary>
        /// <param name="second">秒数。</param>
        /// <returns>毫秒数。</returns>
        protected static double GetSecondToMilliseconds(double second)
        {
            return second.GetSecondToMilliseconds();
        }

        /// <summary>
        /// 将分转换成毫秒。
        /// </summary>
        /// <param name="minute">分。</param>
        /// <returns>毫秒数。</returns>
        protected static double GetMinuteToMilliseconds(double minute)
        {
            return minute.GetMinuteToMilliseconds();
        }

        /// <summary>
        /// 将小时转换成毫秒。
        /// </summary>
        /// <param name="hour">小时。</param>
        /// <returns>毫秒数。</returns>
        protected static double GetHourToMilliseconds(double hour)
        {
            return hour.GetHourToMilliseconds();
        }

        /// <summary>
        /// 将天转换成毫秒。
        /// </summary>
        /// <param name="day">天数。</param>
        /// <returns>毫秒数。</returns>
        protected static double GetDayToMilliseconds(double day)
        {
            return day.GetDayToMilliseconds();
        }

        /// <summary>
        /// 将指定的时间与当前时间的间隔数转换成毫秒。
        /// </summary>
        /// <param name="executeTime">指定的时间。</param>
        /// <returns>毫秒数。</returns>
        protected static double GetDateToMilliseconds(DateTime executeTime)
        {
            return executeTime.GetDateToMilliseconds();
        }

        /// <summary>
        /// 设置时钟周期。
        /// </summary>
        /// <param name="interval">时钟周期。(单位:毫秒)</param>
        public void SetInterval(double interval)
        {
            Timer.Interval = interval;
        }

        /// <summary>
        /// 设置时钟周期。
        /// </summary>
        /// <param name="second">时钟周期。(单位:秒)</param>
        protected void SetIntervalSecond(double second)
        {
            SetInterval(GetSecondToMilliseconds(second));
        }

        /// <summary>
        /// 设置时钟周期。
        /// </summary>
        /// <param name="minute">时钟周期。(单位:分钟)</param>
        protected void SetIntervalMinute(double minute)
        {
            SetInterval(GetMinuteToMilliseconds(minute));
        }

        /// <summary>
        /// 设置时钟周期。
        /// </summary>
        /// <param name="hour">时钟周期。(单位:小时)</param>
        public void SetIntervalHour(double hour)
        {
            SetInterval(GetHourToMilliseconds(hour));
        }

        /// <summary>
        /// 设置时钟周期。
        /// </summary>
        /// <param name="day">时钟周期。(单位:天)</param>
        public void SetIntervalDay(double day)
        {
            SetInterval(GetDayToMilliseconds(day));
        }

        /// <summary>
        /// 设置每天定时触发。
        /// </summary>
        /// <param name="hour">小时。</param>
        /// <param name="minute">分钟。</param>
        /// <param name="second">秒。</param>
        public void SetDayTime(int hour = 0, int minute = 0, int second = 0)
        {
            var runTime = DateTime.Today.AddHours(hour).AddMinutes(minute).AddSeconds(second);

            if (runTime < DateTime.Now)
            {
                runTime = runTime.AddDays(1);
            }

            var span = runTime - DateTime.Now;

            SetInterval(span.TotalMilliseconds);
        }

        /// <summary>
        /// 获得下次处理时间。
        /// </summary>
        /// <returns>下次处理时间。</returns>
        protected virtual string GetNextTime()
        {
            //输出时间。
            var nextTime = DateTime.Now.AddMilliseconds(Timer.Interval);

            return nextTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #endregion 设置时间


        #region 释放资源

        public void Dispose()
        {
            Timer.Elapsed -= Elapsed_Timer;
            Timer.Close();
        }

        #endregion
    }

    /// <summary>
    /// 指定时间执行。
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    [Android.Runtime.Preserve(AllMembers = true)]
    public abstract class BackgroundTaskUseTimeBase<TSource> : BackgroundTaskBase
    {
        #region 字段

        #endregion 字段

        #region 初始化

        /// <inheritdoc />
        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="second">下次任务刷新间隔时间（单位：秒）。</param>
        /// <param name="startSecond">多少秒后启动任务。</param>
        protected BackgroundTaskUseTimeBase(int second, int startSecond = 1) 
            : base(startSecond * 1000)
        {
            var setDeliveryBackgroundTask =
                new SetDeliveryBackgroundTask(second, () => { NextTime = GetNextTaskTime(); });

            setDeliveryBackgroundTask.Start();
        }

        #endregion 初始化

        #region 执行

        /// <inheritdoc />
        /// <summary>
        /// 执行。
        /// </summary>
        protected override void OnExecute()
        {
            SetIntervalMinute(30);

            var defaultTime = DateTime.Now.GetDefaultTime();

            var nextTime = NextTime;

            var isExecute = false;

            if (nextTime > defaultTime && DateTime.Now > nextTime)
            {
                var tasks = GetTasks().ToArray();

                if (tasks.Any())
                {
                    foreach (var task in tasks)
                    {
                        OnExecute(task);
                    }

                    NextTime = DateTime.Now.AddSeconds(10);

                    isExecute = true;
                }

                if (!isExecute)
                {
                    NextTime = GetNextTaskTime();
                }
            }

            SetIntervalSecond(10);
        }

        /// <summary>
        /// 获得可处理的任务集合。
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<TSource> GetTasks();

        /// <summary>
        /// 执行任务处理。
        /// </summary>
        /// <param name="task"></param>
        protected abstract void OnExecute(TSource task);

        #endregion 执行

        #region 定点执行

        /// <summary>
        /// 下次执行时间。
        /// </summary>
        private DateTime _nextTime = DateTime.Now.GetDefaultTime();

        /// <summary>
        /// 下次执行时间。
        /// </summary>
        protected DateTime NextTime
        {
            get => _nextTime;
            set
            {
                var defaultTime = DateTime.Now.GetDefaultTime();

                if (value <= defaultTime)
                    return;

                _nextTime = value;
            }
        }

        /// <summary>
        /// 获得下一个任务的执行时间。
        /// </summary>
        /// <returns></returns>
        protected abstract DateTime GetNextTaskTime();

        #endregion 定点执行

        #region 定时设置任务

        /// <summary>
        /// 设置定时任务。
        /// </summary>
        private class SetDeliveryBackgroundTask : BackgroundTaskBase
        {
            /// <summary>
            /// 时间
            /// </summary>
            private readonly int _second;

            /// <summary>
            /// 动作
            /// </summary>
            private readonly Action _action;

            /// <summary>
            /// 设置下次执行时间任务
            /// </summary>
            /// <param name="second"></param>
            /// <param name="acion"></param>
            public SetDeliveryBackgroundTask(int second, Action acion)
                : base(GetSecondToMilliseconds(12))
            {
                _second = second;
                _action = acion;
            }

            protected override void OnExecute()
            {
                SetIntervalSecond(_second);

                _action();
            }
        }

        #endregion 定时设置任务
    }
}