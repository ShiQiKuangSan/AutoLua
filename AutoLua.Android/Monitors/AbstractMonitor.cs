using System;
using Android.Content;

namespace AutoLua.Droid.Monitors
{
    internal abstract class AbstractMonitor
    {
        protected Context context;

        protected AbstractMonitor(Context context)
        {
            this.context = context;

            Init();
            try
            {
                UnRegister();
            }
            catch (Exception)
            {
            }
            
            Register();
        }


        public abstract void Init();

        public abstract void Register();

        public abstract void UnRegister();
    }
}