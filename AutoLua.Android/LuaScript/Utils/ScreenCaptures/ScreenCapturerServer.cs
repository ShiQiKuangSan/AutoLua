namespace AutoLua.Droid.LuaScript.Utils.ScreenCaptures
{
    /// <summary>
    /// 屏幕截屏服务。
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public sealed class ScreenCapturerServer : ScreenCapturerServerBase
    {
        private static readonly object Lock = new object();

        private static ScreenCapturerServer _instance;

        public static ScreenCapturerServer Instance
        {
            get
            {
                lock (Lock)
                {
                    _instance ??= new ScreenCapturerServer();

                    return _instance;
                }
            }
        }

        private ScreenCapturerServer()
        {
        }

        public override string GetTag()
        {
            return "ScreenCapturerServer";
        }

    }
}