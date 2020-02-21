using System;
using System.Collections.Generic;
using AutoLua.Droid.LuaScript.Utils.Timers;

namespace AutoLua.Droid.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class TimerApi : IDisposable
    {
        private readonly IDictionary<string, TimerTask> _timerTasks = new Dictionary<string, TimerTask>();

        /// <summary>
        /// 预定每隔 delay 毫秒重复执行的 callback。 返回一个用于 clearInterval() 的 id。
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delay"></param>
        public string setInterval(Action action, long delay)
        {
            if (delay <= 0)
            {
                delay = 1;
            }

            var timer = new TimerTask(action, delay, false);

            _timerTasks.Add(timer.Id.ToString(), timer);

            timer.Start();

            return timer.Id.ToString();
        }

        /// <summary>
        /// 预定在 delay 毫秒之后执行的单次 callback。 返回一个用于 clearTimeout() 的 id。
        /// callback 可能不会精确地在 delay 毫秒被调用。 
        /// </summary>
        /// <param name="action">当定时器到点时要调用的函数</param>
        /// <param name="delay">调用 callback 之前要等待的毫秒数</param>
        public void setTimeout(Action action, long delay)
        {
            if (delay <= 0)
            {
                delay = 1;
            }

            var timer = new TimerTask(action, delay);

            timer.Start();
        }

        /// <summary>
        /// 取消一个由 setInterval() 创建的循环定时任务。
        /// </summary>
        /// <param name="id"></param>
        public void clearInterval(string id)
        {
            if (!_timerTasks.ContainsKey(id))
                return;

            var t = _timerTasks[id];

            t?.Stop();
            _timerTasks.Remove(id);
        }

        public void Dispose()
        {
            foreach (var (k, v) in _timerTasks)
            {
                v.Stop();
            }

            _timerTasks.Clear();
        }
    }
}