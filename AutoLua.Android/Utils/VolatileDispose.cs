using Java.Lang;
using NLua.Exceptions;

namespace AutoLua.Droid.Utils
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class VolatileDispose : Object
    {
        private volatile object _value;
        private volatile Thread _thread;

        public VolatileDispose()
        {
            _thread = Thread.CurrentThread();
        }
        public T BlockedGet<T>()
        {
            lock (this)
            {
                if (_value != null)
                {
                    return (T)_value;
                }
                try
                {
#pragma warning disable 618
                    _thread.Suspend();
#pragma warning restore 618
                }
                catch (InterruptedException e)
                {
                    throw new LuaException(e.Message);
                }
            }

            lock (this)
            {
                return (T)_value;
            }
        }

        public T BlockedGetOrThrow<T>()
        {
            lock (this)
            {
                if (_value != null)
                {
                    return (T)_value;
                }
                try
                {
#pragma warning disable 618
                    _thread.Suspend();
#pragma warning restore 618
                }
                catch (Exception e)
                {
                    throw new LuaException(e.Message);
                }
            }

            lock (this)
            {
                return (T)_value;
            }
        }


        public T BlockedGetOrThrow<T>(T defaultValue)
        {
            lock (this)
            {
                if (_value != null)
                {
                    return (T)_value;
                }
                try
                {
#pragma warning disable 618
                    _thread.Suspend();
#pragma warning restore 618
                }
                catch (Exception e)
                {
                    throw new LuaException(e.Message);
                }

                if (_value == null)
                {
                    return defaultValue;
                }
            }

            lock (this)
            {
                return (T)_value;
            }
        }

        public void SetAndNotify<T>(T value)
        {
            _value = value;
#pragma warning disable 618
            _thread.Resume();
#pragma warning restore 618
        }
    }
}