using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Runtime;
using AutoLua.Droid.AutoAccessibility;
using AutoLua.Droid.LuaScript;
using AutoLua.Droid.LuaScript.Api;
using AutoLua.Droid.Utils;
using AutoLua.Events;
using static Android.App.Application;
using Application = Android.App.Application;
using Exception = System.Exception;

namespace AutoLua.Droid
{
    [Application(AllowBackup = true)]
    [Register("AutoLua.Droid.AppApplication")]
    public class AppApplication : Application
    {
        protected AppApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Instance = this;
            ScreenMetrics.Instance.Init();
            
            AppUtils.Init(this);

            this.RegisterActivityLifecycleCallbacks(new SimpleActivityLifecycleCallbacks());

            //初始化无障碍服务
            AutoGlobal.Init(this);
            //初始化lua全局函数
            LuaGlobal.Instance.Init();

            StartServer();
        }

        private void StartServer()
        {
            server = new AccessibilityHttpServer(9091);
            server.SetRoot("/assets/");
            Task.Factory.StartNew(() => server.Start());
        }


        /// <summary>
        /// 输出日志。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void OnLog(string type, string message, Xamarin.Forms.Color color)
        {
            LogEventDelegates.Instance?.OnLog(new LogEventArgs(type, message, color));
        }

        public static T GetSystemService<T>(string service) where T : class, IJavaObject
        {
            var context = Instance;
            var systemService = context.GetSystemService(service).JavaCast<T>();
            if (systemService == null)
            {
                throw new Exception("should never happen..." + service);
            }

            return systemService;
        }
        
        public static AppApplication Instance { get; private set; }

        public static dynamic Lua { get; set; }

        public static System.Threading.Thread LuaThread { get; set; }

        public AccessibilityHttpServer server;
    }

    public class SimpleActivityLifecycleCallbacks : Java.Lang.Object, IActivityLifecycleCallbacks
    {
        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
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