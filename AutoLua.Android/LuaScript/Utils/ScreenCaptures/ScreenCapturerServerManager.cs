using Android.Graphics;
using AutoLua.Droid.Utils;
using System.Threading.Tasks;

namespace AutoLua.Droid.LuaScript.Utils.ScreenCaptures
{
    public class ScreenCapturerServerManager
    {
        /// <summary>
        /// http服务截屏
        /// </summary>
        /// <returns></returns>
        public static Bitmap HttpCapturer()
        {
            var volatileDispose = new VolatileDispose();

            var t = new Task(() =>
            {
                HttpCapturerServer.Instance.Capture(volatileDispose);
            });

            t.Start();
            t.Wait();

            return volatileDispose.blockedGet<Bitmap>();
        }

        /// <summary>
        /// lua引擎截屏。
        /// </summary>
        /// <returns></returns>
        public static Bitmap LuaCapturer()
        {
            var volatileDispose = new VolatileDispose();

            var t = new Task(() =>
            {
                ScreenCapturerServer.Instance.Capture(volatileDispose);
            });

            t.Start();
            t.Wait();

            return volatileDispose.blockedGet<Bitmap>();
        }
    }
}