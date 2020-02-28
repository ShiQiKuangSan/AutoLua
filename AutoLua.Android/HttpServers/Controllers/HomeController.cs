using Android.Graphics;
using AutoLua.Droid.LuaScript.Utils.ScreenCaptures;
using HttpServer.Modules;
using System;
using System.IO;

namespace AutoLua.Droid.HttpServers.Controllers
{
    /// <summary>
    /// 首页控制器。
    /// </summary>
    public class HomeController : Controller
    {
        [Route(RouteMethod.GET, "/")]
        public ActionResult Index()
        {
            try
            {
                var bitmap = ScreenCapturerServerManager.HttpCapturer();

                if (bitmap == null)
                {
                    return new ActionResult("没有截到图", null, false);
                }

                using var outputStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, outputStream);
                var imageBytes = outputStream.ToArray();

                var base64 = Convert.ToBase64String(imageBytes);
                base64 = "data:image/png;base64," + base64;

                var html = $@"<!DOCTYPE html>
                            <html>
                            	<head>
                            		<meta charset='utf - 8'>
                                    <title>手机图片</title>
                                </head>
                                <body>
                                       <img src='{base64}'/>
                                </body>
                            </html>
                ";

                return new ActionResult("", html);
            }
            catch (Exception e)
            {
                return new ActionResult(e.Message, null, false);
            }

        }

        [Route(RouteMethod.GET, "/")]
        public ActionResult Index(int qq)
        {
            return new ActionResult("", $"我是 参数: {nameof(qq)} ,{qq}");
        }
    }
}