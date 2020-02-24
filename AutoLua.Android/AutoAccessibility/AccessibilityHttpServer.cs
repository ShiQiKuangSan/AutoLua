using AutoLua.Droid.Utils;
using HttpServer;
using System.IO;
using System.Linq;

namespace AutoLua.Droid.AutoAccessibility
{
    public class AccessibilityHttpServer : HttpServer.HttpServer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">IP地址</param>
        public AccessibilityHttpServer(int port)
            : base(AppUtils.GetIp(), port)
        {

        }

        public override void OnPost(HttpRequest request, HttpResponse response)
        {
            //获取客户端传递的参数
            string data = request.Params == null ? "" : string.Join(";", request.Params.Select(x => x.Key + "=" + x.Value).ToArray());

            //设置返回信息
            string content = string.Format("这是通过Post方式返回的数据:{0}", data);

            //构造响应报文
            response.SetContent(content);
            response.Content_Encoding = "utf-8";
            response.StatusCode = "200";
            response.Content_Type = "text/html; charset=UTF-8";
            response.Headers["Server"] = "ExampleServer";

            response.Send();

        }

        public override void OnGet(HttpRequest request, HttpResponse response)
        {
            ///链接形式1:"http://localhost:4050/assets/styles/style.css"表示访问指定文件资源，
            ///此时读取服务器目录下的/assets/styles/style.css文件。

            ///链接形式1:"http://localhost:4050/assets/styles/"表示访问指定页面资源，
            ///此时读取服务器目录下的/assets/styles/style.index文件。

            //当文件不存在时应返回404状态码
            string requestURL = request.URL;
            requestURL = requestURL.Replace("/", @"\").Replace("\\..", "").TrimStart('\\');
            string requestFile = Path.Combine(ServerRoot, requestURL);

            //判断地址中是否存在扩展名
            string extension = Path.GetExtension(requestFile);

            //发送HTTP响应
            response.Send();
        }
    }
}