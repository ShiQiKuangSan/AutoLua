using Android.App;
using Android.Util;

namespace AutoLua.Droid.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ScreenMetrics
    {
        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool initialized = false;

        public int DeviceScreenHeight { get; private set; }
        public int DeviceScreenWidth { get; private set; }

        public DisplayMetricsDensity DeviceScreenDensity { get; private set; }

        private readonly static object Lock = new object();

        private static ScreenMetrics instance = null;

        public static ScreenMetrics Instance
        {
            get
            {
                lock (Lock)
                {
                    if (instance == null) instance = new ScreenMetrics();

                    return instance;
                }
            }
        }

        public void Init(Activity activity)
        {
            if (initialized)
                return;

            var metrics = new DisplayMetrics();
            activity.WindowManager.DefaultDisplay.GetRealMetrics(metrics);
            DeviceScreenHeight = metrics.HeightPixels;
            DeviceScreenWidth = metrics.WidthPixels;
            DeviceScreenDensity = metrics.DensityDpi;
            initialized = true;
        }

        /// <summary>
        /// 方向感知屏幕宽度
        /// </summary>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public int GetOrientationAwareScreenWidth(Android.Content.Res.Orientation orientation)
        {
            if (orientation == Android.Content.Res.Orientation.Landscape)
            {
                return DeviceScreenHeight;
            }
            else
            {
                return DeviceScreenWidth;
            }
        }

        /// <summary>
        /// 方向感知屏幕高度
        /// </summary>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public int GetOrientationAwareScreenHeight(Android.Content.Res.Orientation orientation)
        {
            if (orientation == Android.Content.Res.Orientation.Landscape)
            {
                return DeviceScreenWidth;
            }
            else
            {
                return DeviceScreenHeight;
            }
        }
    }
}