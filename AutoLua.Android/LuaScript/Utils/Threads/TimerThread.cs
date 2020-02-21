using System;
using System.Threading;
using AutoLua.Droid.LuaScript.Utils.Timers;

namespace AutoLua.Droid.LuaScript.Utils.Threads
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class TimerThread : IDisposable
    {
        private Thread _thread;
        private TimerTask _timer;
        private static readonly object Lock = new object();
        private static bool _isRun;

        public string name
        {
            get => _thread.Name;
            internal set => _thread.Name = value;
        }

        public TimerThread(Action action)
        {
            _thread = new Thread(() =>
            {
                lock (Lock)
                {
                    _isRun = true;
                    action();
                }
            });
        }


        /// <summary>
        /// 启动线程。
        /// </summary>
        internal void start()
        {
            _thread.Start();
        }

        /// <summary>
        /// 中断线程运行。
        /// </summary>
        public void interrupt()
        {
            _thread.Interrupt();
        }

        /// <summary>
        /// 等待线程执行完成。如果timeout为0，则会一直等待直至该线程执行完成；否则最多等待timeout毫秒的时间。
        /// </summary>
        /// <param name="millisecondsTimeout">等待时间，单位毫秒</param>
        public void join(int millisecondsTimeout = 0)
        {
            if (millisecondsTimeout <= 0)
            {
                _thread.Join();
            }
            else
            {
                _thread.Join(millisecondsTimeout);
            }
        }

        /// <summary>
        /// 返回线程是否存活。如果线程仍未开始或已经结束，返回false; 如果线程已经开始或者正在运行中，返回true。
        /// </summary>
        /// <returns></returns>
        public bool isAlive()
        {
            return _thread.IsAlive;
        }

        /// <summary>
        /// 等待线程开始执行。调用threads.start()以后线程仍然需要一定时间才能开始执行，因此调用此函数会等待线程开始执行；如果线程已经处于执行状态则立即返回。
        /// </summary>
        public void waitFor()
        {
            lock (Lock)
            {
                while (!_isRun)
                {
                    Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        ///  设置定时任务。该任务只执行一次
        /// </summary>
        /// <param name="action">回调函数 function() end</param>
        /// <param name="delay">时间</param>
        public void setTimeout(Action action, long delay)
        {
            StopTime();
            _timer = new TimerTask(action, delay)
            {
                Name = name
            };

            _timer.Start();
        }

        /// <summary>
        /// 设置定时任务。无限循环
        /// </summary>
        /// <param name="action">回调函数 function() end</param>
        /// <param name="delay">时间</param>
        public void setInterval(Action action, long delay)
        {
            StopTime();
            _timer = new TimerTask(action, delay, false)
            {
                Name = name
            };

            _timer.Start();
        }

        /// <summary>
        /// 清除定时器。
        /// </summary>
        public void clearInterval()
        {
            StopTime();
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _thread.Interrupt();
            _thread = null;

            StopTime();
        }

        /// <summary>
        /// 停止定时器。
        /// </summary>
        private void StopTime()
        {
            _timer?.Stop();
            _timer = null;
        }
    }
}