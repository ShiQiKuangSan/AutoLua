using Android.Graphics;
using Android.Runtime;
using AutoLua.Droid.HttpServers.Models;
using AutoLua.Droid.LuaScript.Utils.ScreenCaptures;
using AutoLua.Droid.Utils;
using HttpServer.Modules;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AutoLua.Droid.HttpServers.Controllers
{
    /// <summary>
    /// 首页控制器。
    /// </summary>
    [Preserve(AllMembers = true)]
    public class HomeController : Controller
    {
        private readonly string _serverUrl;

        public HomeController()
        {
            _serverUrl = $"http://{AppUtils.GetIp()}:{AppApplication.HttpServerPort}/";
        }

        [Route(RouteMethod.GET, "/")]
        public ActionResult Index()
        {
            const string path = "Site";
            var fileHtml = AppUtils.GetAssets.List(path);

            try
            {
                var file = fileHtml.Where(x => x.EndsWith(".html")).FirstOrDefault(x => x == "index.html");

                if (string.IsNullOrWhiteSpace(file))
                {
                    return Html("页面不存在");
                }

                var f = $"{path}/{file}";

                using var sr = new StreamReader(AppUtils.GetAssets.Open(f));

                var str = sr.ReadToEnd();

                if (string.IsNullOrWhiteSpace(str))
                {
                    return Html("页面不存在001");
                }

                str = str.Replace("{{WEBSERVER}}", _serverUrl);

                return Html(str);
            }
            catch (Exception e)
            {
                return Html(e.Message);
            }
        }

        [Route(RouteMethod.GET, "/demo")]
        public ActionResult Demo(int q)
        {
            return JsonSuccess("");
        }
    }
}