using System;
using System.Collections.Generic;
using System.Threading;
using Android.Runtime;
using AutoLua.Core.Common;
using AutoLua.Core.LuaScript.ApiCommon.Threads;
using Java.Util.Concurrent.Atomic;
using Java.Util.Concurrent.Locks;
using NLua.Exceptions;

namespace AutoLua.Core.LuaScript.Api
{
    [Preserve(AllMembers = true)]
    public class ThreadApi
    {
        private readonly HashSet<TimerThread> _threads = new HashSet<TimerThread>();
        private readonly Thread _mainThread;
        private bool _isExit;

        public ThreadApi()
        {
            _mainThread = Thread.CurrentThread;
        }

        /// <summary>
        /// 启动一个线程
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <exception cref="LuaException"></exception>
        public TimerThread start(Action action)
        {
            var t = new TimerThread(action);
            lock (_threads)
            {
                if (_isExit)
                {
                    throw new LuaException("脚本已退出");
                }

                _threads.Add(t);
                t.name = $"{_mainThread.Name} (产生线程数-{_threads.Count})";
                t.start();
            }

            return t;
        }

        /// <summary>
        /// 停止所有通过threads.start()启动的子线程。
        /// </summary>
        public void shutDownAll()
        {
            lock (_threads)
            {
                foreach (var thread in _threads)
                {
                    thread.interrupt();
                }

                _threads.Clear();
            }
        }

        /// <summary>
        /// 返回当前线程。
        /// </summary>
        /// <returns></returns>
        public Thread currentThread()
        {
            var t = Thread.CurrentThread;
            return t;
        }

        /// <summary>
        /// 新建一个Disposable对象，用于等待另一个线程的某个一次性结果。更多信息参见线程通信以及Disposable。
        /// </summary>
        /// <returns></returns>
        public VolatileDispose disposable()
        {
            return new VolatileDispose();
        }

        /// <summary>
        /// 新建一个整数原子变量。更多信息参见线程安全以及AtomicLong。
        /// </summary>
        /// <param name="value">初始整数值，默认为0</param>
        /// <returns></returns>
        public AtomicLong atomic(long value = 0)
        {
            return new AtomicLong(value);
        }

        /// <summary>
        /// 新建一个可重入锁。更多信息参见线程安全以及ReentrantLock。
        /// </summary>
        /// <returns></returns>
        public ReentrantLock locks()
        {
            return new ReentrantLock();
        }

        public void exit()
        {
            lock (_threads)
            {
                shutDownAll();
                _isExit = true;
            }
        }
    }
}