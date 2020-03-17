using AutoLua.Core.Common;

namespace AutoLua.Droid.HttpServers
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class WebHttpServer : HttpServer.HttpService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">IP地址</param>
        public WebHttpServer(int port)
            : base(AppUtils.GetIp(), port)
        {

        }
    }
}