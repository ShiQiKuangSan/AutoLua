using Android.Graphics;
using AutoLua.Droid.LuaScript.Utils.ScreenCaptures;
using AutoLua.Droid.Utils;
using HttpServer;
using System;
using System.IO;
using System.Linq;
using Path = System.IO.Path;

namespace AutoLua.Droid.AutoAccessibility
{
    public class AccessibilityHttpServer : HttpServer.HttpServer
    {
        private static readonly object Lock = new object();

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

            //当文件不存在时应返回404状态码
            var requestURL = request.URL;
            requestURL = requestURL.Replace("/", @"\").Replace("\\..", "").TrimStart('\\');

            var requestFile = Path.Combine(ServerRoot, requestURL);

            try
            {
                var bitmap = ScreenCapturerServerManager.HttpCapturer();

                if (bitmap != null)
                {
                    using var outputStream = new MemoryStream();
                    bitmap.Compress(Bitmap.CompressFormat.Png, 100, outputStream);
                    var imageBytes = outputStream.ToArray();

                    var base64 = Convert.ToBase64String(imageBytes);
                    base64 = "data:image/png;base64," + base64;
                    response.SetContent($@"<!DOCTYPE html>
<html>
	<head>
		<meta charset='utf - 8'>
        < title ></ title >

    </ head >

    < body >
           <img src='{base64}'/>
    </ body >
</ html >
");

                    response.Content_Type = "text/html; charset=UTF-8";
                }
                else
                {
                    response.SetContent("没有截到图");
                    response.Content_Type = "text/html; charset=UTF-8";
                }
            }
            catch (System.Exception e)
            {
                response.SetContent(e.Message);
                response.Content_Type = "text/html; charset=UTF-8";
            }

            response.StatusCode = "200";
            //发送HTTP响应
            response.Send();
        }
    }
}