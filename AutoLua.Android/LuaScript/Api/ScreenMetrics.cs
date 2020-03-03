using Android.Content.Res;
using Android.Util;

namespace AutoLua.Droid.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ScreenMetrics
    {
        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool _initialized;

        public int DeviceScreenHeight { get; private set; }
        public int DeviceScreenWidth { get; private set; }

        public DisplayMetricsDensity DeviceScreenDensity { get; private set; }

        private static readonly object Lock = new object();

        private static ScreenMetrics _instance = null;

        public static ScreenMetrics Instance
        {
            get
            {
                lock (Lock)
                {
                    return _instance ??= new ScreenMetrics();
                }
            }
        }

        public void Init()
        {
            if (_initialized)
                return;

            var metrics = Resources.System.DisplayMetrics;
            DeviceScreenHeight = metrics.HeightPixels;
            DeviceScreenWidth = metrics.WidthPixels;
            DeviceScreenDensity = metrics.DensityDpi;
            _initialized = true;
        }

        /// <summary>
        /// 方向感知屏幕宽度
        /// </summary>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public int GetOrientationAwareScreenWidth(Android.Content.Res.Orientation orientation)
        {
            return orientation == Android.Content.Res.Orientation.Landscape ? DeviceScreenHeight : DeviceScreenWidth;
        }

        /// <summary>
        /// 方向感知屏幕高度
        /// </summary>
        /// <param name="orientation"></param>
        /// <returns></returns>
        public int GetOrientationAwareScreenHeight(Android.Content.Res.Orientation orientation)
        {
            return orientation == Android.Content.Res.Orientation.Landscape ? DeviceScreenWidth : DeviceScreenHeight;
        }
    }
}