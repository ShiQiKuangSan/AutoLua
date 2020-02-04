using System;
using System.Text;
using Android.App;
using Android.OS;
using Android.Runtime;
using AutoLua.Droid.AutoAccessibility;
using AutoLua.Droid.LuaScript;
using AutoLua.Droid.LuaScript.Api;
using AutoLua.Droid.Utils;
using AutoLua.Events;
using Java.Lang;
using NLua;
using static Android.App.Application;
using Application = Android.App.Application;

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
            
            //注册全局异常捕获事件。
            AndroidEnvironment.UnhandledExceptionRaiser += UnhandledExceptionHandler;
          
            AppUtils.Init(this);
            InitLua();
            this.RegisterActivityLifecycleCallbacks(new SimpleActivityLifecycleCallbacks());

            //初始化无障碍服务
            AutoGlobal.Init(this);
            //初始化lua全局函数
            LuaGlobal.Instance.Init();
        }

        protected override void Dispose(bool disposing)
        {
            AndroidEnvironment.UnhandledExceptionRaiser -= UnhandledExceptionHandler;
            base.Dispose(disposing);
        }

        /// <summary>
        /// 一个全局的异常处理操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UnhandledExceptionHandler(object sender, RaiseThrowableEventArgs e)
        {
            ToastUtils.ShowSingleLongToast(e.Exception.Message);
            LogEventDelegates.Instance.OnLog(new LogEventArgs("异常", e.Exception.Message, Xamarin.Forms.Color.Red));
        }

        private static void InitLua()
        {
            Lua = new Lua();
            Lua.State.Encoding = Encoding.UTF8;
            Lua.HookException += Lua_HookException;
        }

        private static void Lua_HookException(object sender, NLua.Event.HookExceptionEventArgs e)
        {
            LogEventDelegates.Instance?.OnLog(new LogEventArgs("异常", e.Exception.Message, Xamarin.Forms.Color.Red));
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
                throw new RuntimeException("should never happen..." + service);
            }

            return systemService;
        }
        
        public static AppApplication Instance { get; private set; }

        public static Lua Lua { get; private set; }

        public static System.Threading.Thread LuaThread { get; set; }
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