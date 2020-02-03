namespace AutoLua.Droid.Http
{
    /// <summary>
    /// Http请求管理器
    /// </summary>
    public static class HttpClientManager
    {
        private static readonly object Lock = new object();

        private static HttpClient _client;


        /// <summary>
        /// 当前任务。
        /// </summary>
        public static HttpClient Current
        {
            get
            {
                lock (Lock)
                {
                    return _client ??= new HttpClient();
                }
            }
        }

        /// <summary>
        /// 生成一个新的Http请求类。
        /// </summary>
        public static HttpClient NewHttpClient()
        {
            _client = new HttpClient();
            return _client;
        }

        /// <summary>
        /// 重置默认的Http请求工具。
        /// </summary>
        /// <returns></returns>
        public static HttpClient ResetHttpClient()
        {
            return NewHttpClient();
        }
    }
}