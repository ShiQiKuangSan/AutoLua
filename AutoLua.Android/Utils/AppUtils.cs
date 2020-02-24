using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using Java.Net;
using System;

namespace AutoLua.Droid.Utils
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class AppUtils
    {
        private static Thread _uiThread;

        private static readonly Handler UiHandler = new Handler(Looper.MainLooper);
        
        private static readonly Handler Handler = new Handler();

        private static volatile Java.Lang.Ref.WeakReference _currentActivity = new Java.Lang.Ref.WeakReference(null);

        public static void Init(Context context)
        {
            GetAppContext = context;
            _uiThread = Thread.CurrentThread();
        }

        public static Context GetAppContext { get; private set; }

        public static AssetManager GetAssets => GetAppContext.Assets;

        public static Resources GetResource => GetAppContext.Resources;

        public static bool IsUiThread => Thread.CurrentThread() == _uiThread;

        public static void RunOnUI(Action r) => UiHandler.Post(r);

        public static void Run(Action r) => Handler.Post(r);

        public static void RunOnUIDelayed(Action r, long delayMills) => UiHandler.PostDelayed(r, delayMills);

        public static void RemoveRunnable(Action r)
        {
            if (r == null)
            {
                UiHandler.RemoveCallbacksAndMessages(null);
            }
            else
            {
                UiHandler.RemoveCallbacks(r);
            }
        }

        public static void SetCurrentActivity(Activity currentActivity)
        {
            if (AppApplication.Lua!=null)
            {
                AppApplication.Lua.activity = currentActivity;
            }

            _currentActivity = new Java.Lang.Ref.WeakReference(currentActivity);
        }

        public static Activity CurrentActivity => _currentActivity.Get() as Activity;

        /// <summary>
        /// 获取本地ip
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            try
            {
                var work = NetworkInterface.NetworkInterfaces;

                while (work.HasMoreElements)
                {
                    var netAddress = work.NextElement().JavaCast<NetworkInterface>().InetAddresses; 

                    while (netAddress.HasMoreElements)
                    {
                        var net = netAddress.NextElement().JavaCast<InetAddress>();
                        if (!net.IsLoopbackAddress && (net is Inet4Address))
                        {
                            return net.HostAddress.ToString();
                        }
                    }
                }
            }
            catch (System.Exception)
            {
            }

            return string.Empty;
        }
    }
}