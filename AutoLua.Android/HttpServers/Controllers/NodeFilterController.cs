using Android.Runtime;
using HttpServer.Modules;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using AutoLua.Core.Extensions;
using AutoLua.Droid.Utils;

namespace AutoLua.Droid.HttpServers.Controllers
{
    /// <summary>
    /// 节点过滤控制器
    /// </summary>
    [Preserve(AllMembers = true)]
    public class NodeFilterController : Controller
    {
        /// <summary>
        /// 查找节点，返回单个节点
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/by/findOne")]
        public ActionResult FindOneV1(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return JsonError("json不能为空");
            }

            try
            {
                var obj = JObject.Parse(json);

                var by = NodeHelper.FilterJson(obj);
                var root = by.findOnce();

                if (root == null)
                {
                    return JsonSuccess();
                }

                var model = root.To();

                return model == null ? JsonSuccess() : JsonSuccess(model);
            }
            catch (Exception e)
            {
                return JsonError($"json 格式错误 .. {e.Message}");
            }
        }

        /// <summary>
        /// 查找节点，返回节点集合
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [Route(RouteMethod.GET, "/api/v1/by/find")]
        public ActionResult FindV1(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return JsonError("json不能为空");
            }

            try
            {
                var obj = JObject.Parse(json);

                var by = NodeHelper.FilterJson(obj);

                var roots = by.find();

                if (!roots.Any())
                {
                    return JsonSuccess();
                }

                var list = roots.Select(x => x.To()).ToList();

                return JsonSuccess(list);
            }
            catch (Exception e)
            {
                return JsonError($"json 格式错误 .. {e.Message}");
            }
        }
    }
}