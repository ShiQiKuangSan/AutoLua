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
            //创建一个线程操作类，构造器会自动获取当前使用的线程，
            var volatileDispose = new VolatileDispose();

            var t = new Task(() =>
            {
                //执行截屏的时候会挂起 volatileDispose 的线程
                HttpCapturerServer.Instance.Capture(volatileDispose);
            });

            t.Start();
            t.Wait();

            //等待 volatileDispose 线程恢复并获取 返回的值。
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