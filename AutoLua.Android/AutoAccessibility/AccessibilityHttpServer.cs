using AutoLua.Droid.Utils;

namespace AutoLua.Droid.AutoAccessibility
{
    public class AccessibilityHttpServer : HttpServer.HttpService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">IP地址</param>
        public AccessibilityHttpServer(int port)
            : base(AppUtils.GetIp(), port)
        {

        }
    }
}