using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AutoLua.Core.AutoAccessibility;
using AutoLua.Core.Common;
using AutoLua.Core.LuaScript;
using AutoLua.Core.LuaScript.Api;
using DSoft.Messaging;
using Application = Android.App.Application;

namespace AutoLua.Droid
{
    [Application(AllowBackup = true, Label = "AutoLua", SupportsRtl = true)]
    [Register("AutoLua.Droid.AppApplication")]
    public class AppApplication : Application
    {
        protected AppApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            AppUtils.Init(this);

            RegisterActivityLifecycleCallbacks(new SimpleActivityLifecycleCallbacks());

            //初始化无障碍服务
            AutoGlobal.Init(this);
            //初始化lua全局函数
            LuaGlobal.Instance.Init();

            //注册启动activity的服务
            MessageBus.Default.Register(new StartActivityMessage("StartActivityMessage", StartActivity));
        }

        public const int HttpServerPort = 8060;


        public void StartActivity(object sender, MessageBusEvent evnt)
        {
            if (evnt.Data?.Length > 0)
            {
                var activityName = evnt.Data[0].ToString();

                Intent i = null;

                switch (activityName)
                {
                    case "MainActivity":
                        i = new Intent(this, typeof(MainActivity));
                        break;
                    default:
                        break;
                }

                if (i != null)
                {
                    base.StartActivity(i);
                }
            }
        }

        private class SimpleActivityLifecycleCallbacks : Java.Lang.Object, IActivityLifecycleCallbacks
        {
            public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
            {
                ScreenMetrics.Instance.Init(activity);
                AppUtils.SetCurrentActivity(activity);
            }

            public void OnActivityDestroyed(Activity activity)
            {
            }

            public void OnActivityPaused(Activity activity)
            {
            }

            public void OnActivityResumed(Activity activity)
            {
                AppUtils.SetCurrentActivity(activity);
            }

            public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
            {
            }

            public void OnActivityStarted(Activity activity)
            {
            }

            public void OnActivityStopped(Activity activity)
            {
            }
        }

        /// <summary>
        /// 活动启动消息事件
        /// </summary>
        public class StartActivityMessage : MessageBusEventHandler
        {
            public StartActivityMessage(string eventId, Action<object, MessageBusEvent> action)
                : base(eventId, action)
            {

            }
        }
    }
}