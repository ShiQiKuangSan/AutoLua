using System.Threading.Tasks;
using Android.Graphics;
using Android.Runtime;

namespace AutoLua.Core.LuaScript.ApiCommon.ScreenCaptures
{
    [Preserve(AllMembers = true)]
    public class ScreenCapturerServerManager
    {
        /// <summary>
        /// lua引擎截屏。
        /// </summary>
        /// <returns></returns>
        public static Bitmap Capturer()
        {
            ScreenCapturerServer.Instance.Capture();

            var bitmap = ScreenCapturerServer.Instance.GetBitmap();

            return bitmap;
        }
    }
}