using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Java.Lang;
using System;

namespace AutoLua.Droid.Utils
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class AppUtils
    {
        private static Thread mUiThread;

        private static Handler _handler = new Handler(Looper.MainLooper);

        private static volatile Java.Lang.Ref.WeakReference _currentActivity = new Java.Lang.Ref.WeakReference(null);

        public static void Init(Context context)
        {
            GetAppContext = context;
            mUiThread = Thread.CurrentThread();
        }

        public static Context GetAppContext { get; private set; }

        public static AssetManager GetAssets => GetAppContext.Assets;

        public static Resources GetResource => GetAppContext.Resources;

        public static bool IsUIThread => Thread.CurrentThread() == mUiThread;

        public static void RunOnUI(Action r) => _handler.Post(r);

        public static void RunOnUIDelayed(Action r, long delayMills) => _handler.PostDelayed(r, delayMills);

        public static void RemoveRunnable(Action r)
        {
            if (r == null)
            {
                _handler.RemoveCallbacksAndMessages(null);
            }
            else
            {
                _handler.RemoveCallbacks(r);
            }
        }

        public static void SetCurrentActivity(Activity currentActivity)
        {
            _currentActivity = new Java.Lang.Ref.WeakReference(currentActivity);
        }

        public static Activity CurrentActivity => _currentActivity.Get() as Activity;
    }
}