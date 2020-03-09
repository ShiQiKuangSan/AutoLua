using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using AutoLua.Core.AutoAccessibility;
using AutoLua.Core.Common;
using AutoLua.Core.LuaScript;
using AutoLua.Core.LuaScript.Api;
using static Android.App.Application;
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
        }

        public const int HttpServerPort = 8060;
    }

    public class SimpleActivityLifecycleCallbacks : Java.Lang.Object, IActivityLifecycleCallbacks
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
}