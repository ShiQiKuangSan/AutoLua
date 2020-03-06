using System.Threading.Tasks;
using Android.Graphics;
using AutoLua.Droid.Utils;

namespace AutoLua.Droid.LuaScript.Utils.ScreenCaptures
{
    public class ScreenCapturerServerManager
    {
        /// <summary>
        /// lua引擎截屏。
        /// </summary>
        /// <returns></returns>
        public static Bitmap Capturer()
        {
            var volatileDispose = new VolatileDispose();

            var t = new Task(() =>
            {
                ScreenCapturerServer.Instance.Capture(volatileDispose);
            });

            t.Start();

            return volatileDispose.blockedGet<Bitmap>();
        }
    }
}