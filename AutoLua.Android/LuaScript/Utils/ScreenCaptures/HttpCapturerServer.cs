namespace AutoLua.Droid.LuaScript.Utils.ScreenCaptures
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public sealed class HttpCapturerServer : ScreenCapturerServerBase
    {
        private static readonly object Lock = new object();

        private static HttpCapturerServer _instance;

        public static HttpCapturerServer Instance
        {
            get
            {
                lock (Lock)
                {
                    _instance ??= new HttpCapturerServer();

                    return _instance;
                }
            }
        }

        private HttpCapturerServer() { }

        public override string GetTag()
        {
            return "HttpCapturerServer";
        }
    }
}