using Java.Lang;
using NLua.Exceptions;

namespace AutoLua.Droid.Utils
{
    public class VolatileDispose : Object
    {
        private volatile object _value;
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
                    AppApplication.LuaThread.Suspend();
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
                    AppApplication.LuaThread.Suspend();
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


        public T BlockedGetOrThrow<T>(LuaException exception, long timeout, T defaultValue)
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
                    AppApplication.LuaThread.Suspend();
#pragma warning restore 618
                }
                catch (InterruptedException e)
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
            AppApplication.LuaThread.Resume();
#pragma warning restore 618
        }
    }
}