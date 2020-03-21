using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AutoLua.Core.AutoAccessibility;
using AutoLua.Core.Common;
using AutoLua.Core.Logging;
using AutoLua.Core.LuaScript;
using AutoLua.Core.LuaScript.Api;
using AutoLua.Droid.Utils;
using AutoLua.Droid.Utils.Logging;
using DSoft.Messaging;
using Java.IO;
using Java.Lang;
using static Java.Lang.Thread;
using Application = Android.App.Application;

namespace AutoLua.Droid
{
    [Application(AllowBackup = false, LargeHeap = true, Label = "AutoLua", SupportsRtl = true)]
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

            LoggerFactory.SetLoggerFactory(new TxtLoggerFactory());

            CrashHandler.Instance.Init(ApplicationContext);

            //注册启动activity的服务
            MessageBus.Default.Register(new StartActivityMessage("StartActivityMessage", StartActivity));

            //初始化lua全局函数
            LuaGlobal.Instance.Init();
        }

        public const int HttpServerPort = 8060;

        /// <summary>
        /// 日志路径
        /// </summary>
        public static readonly string LogPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/autolua_logs/";


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
                    i.SetFlags(ActivityFlags.NewTask);
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

        /// <summary>
        /// 全局的异常捕获
        /// </summary>
        private class CrashHandler : Java.Lang.Object, IUncaughtExceptionHandler
        {
            private const string TAG = "CrashHandler";

            private static CrashHandler _instance = null;

            private static readonly object Lock = new object();

            public static CrashHandler Instance
            {
                get
                {
                    if (_instance != null)
                        return _instance;

                    lock (Lock)
                    {
                        _instance = new CrashHandler();
                        return _instance;
                    }
                }
            }

            private Context _context;

            private readonly Dictionary<string, string> infos = new Dictionary<string, string>();

            private IUncaughtExceptionHandler _uncaughtException;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="context"></param>
            public void Init(Context context)
            {
                _context = context;
                _uncaughtException = DefaultUncaughtExceptionHandler;
                DefaultUncaughtExceptionHandler = this;
            }

            /// <summary>
            /// 全局的异常捕获
            /// </summary>
            /// <param name="thread"></param>
            /// <param name="ex"></param>
            public void UncaughtException(Thread thread, Throwable ex)
            {
                var status = HandleException(ex);
                if (!status && _uncaughtException != null)
                {
                    //如果用户没有处理则让系统默认的异常处理器来处理  
                    _uncaughtException.UncaughtException(thread, ex);
                }
                else
                {
                    //释放资源不能像常规的那样在activity的onDestroy方法里面执行，因为如果出现全局异常捕获，activity的关闭有时候是不会再走相关的生命周期函数的（onDesktroy,onStop,onPause等）。  
                    AppManager.Instance.AppExit(_context);
                }
            }

            /// <summary>
            /// 自定义错误处理，手机错误信息。
            /// </summary>
            /// <returns></returns>
            private bool HandleException(Throwable ex)
            {
                if (ex == null)
                    return false;

                //手机设备参数
                CollectDeviceInfo(_context);
                SaveCrashInfo2File(ex);
                return true;

            }

            /// <summary>
            /// 手机设备参数信息
            /// </summary>
            /// <param name="ctx"></param>
            private void CollectDeviceInfo(Context ctx)
            {
                try
                {
                    var pm = ctx.PackageManager;
                    var pi = pm.GetPackageInfo(ctx.PackageName, PackageInfoFlags.Activities);

                    if (pi != null)
                    {
                        var versionName = pi.VersionName ?? "null";
                        var versionCode = pi.VersionCode;
                        infos.Add("versionName", versionName);
                        infos.Add("versionCode", versionCode.ToString());
                    }
                }
                catch (System.Exception e)
                {
                    infos.Add($"{TAG} PackageManager", e.Message);
                }

                var fields = Class.FromType(typeof(Build)).GetDeclaredFields();

                foreach (var field in fields)
                {
                    try
                    {
                        field.Accessible = true;
                        infos.Add(field.Name, field.Get(null).ToString());
                    }
                    catch (System.Exception e)
                    {
                        infos.Add($"{TAG} field.Accessible", e.Message);
                    }
                }
            }

            /// <summary>
            /// 保存错误信息到文件中
            /// </summary>
            /// <param name="ex"></param>
            private void SaveCrashInfo2File(Throwable ex)
            {
                var sb = new System.Text.StringBuilder();
                foreach (var (key, value) in infos)
                {
                    sb.AppendLine($"{key} = {value}");
                }

                using var writer = new StringWriter();
                using var printWriter = new PrintWriter(writer);

                ex.PrintStackTrace(printWriter);

                var cause = ex.Cause;

                while (cause != null)
                {
                    cause.PrintStackTrace(printWriter);
                    cause = cause.Cause;
                }

                var result = writer.ToString();
                sb.AppendLine(result);

                LoggerFactory.Current.Create().LogError(sb.ToString());
            }
        }

    }
}