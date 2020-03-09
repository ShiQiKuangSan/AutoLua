using System;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using Exception = System.Exception;

namespace AutoLua.Core.Common
{
    [Preserve(AllMembers = true)]
    public static class AppUtils
    {
        private static Thread _uiThread;

        private static readonly Handler UiHandler = new Handler(Looper.MainLooper);

        private static readonly Handler Handler = new Handler();

        private static volatile Java.Lang.Ref.WeakReference _currentActivity = new Java.Lang.Ref.WeakReference(null);

        public static void Init(Context context)
        {
            AppContext = context;
            _uiThread = Thread.CurrentThread();
        }

        public static Context AppContext { get; private set; }

        public static AssetManager GetAssets => AppContext.Assets;

        public static Resources GetResource => AppContext.Resources;

        public static bool IsUiThread => Thread.CurrentThread() == _uiThread;

        public static void RunOnUI(Action r) => UiHandler.Post(r);

        public static void Run(Action r) => Handler.Post(r);

        public static void RunOnUIDelayed(Action r, long delayMills) => UiHandler.PostDelayed(r, delayMills);

        public static dynamic Lua;

        public static System.Threading.Thread LuaThread;
        
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
            if (Lua != null)
            {
                Lua.activity = currentActivity;
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
                var wifiManager = GetSystemService<WifiManager>(Context.WifiService);
                var ip = wifiManager.ConnectionInfo.IpAddress;
                var ipStr = (ip & 0xFF) + "." + ((ip >> 8) & 0xFF) + "." + ((ip >> 16) & 0xFF) + "." + ((ip >> 24) & 0xFF);
                return ipStr;
            }
            catch (System.Exception)
            {
                // ignored
            }

            return string.Empty;
        }
        
        /// <summary>
        /// 获得服务。
        /// </summary>
        /// <param name="service"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T GetSystemService<T>(string service) where T : class, IJavaObject
        {
            var systemService = AppContext.GetSystemService(service).JavaCast<T>();
            if (systemService == null)
            {
                throw new Exception("should never happen..." + service);
            }

            return systemService;
        }
    }
}
