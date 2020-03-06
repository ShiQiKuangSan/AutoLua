﻿using HttpServer.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpServer
{
    public class HttpService : HttpServer
    {
        private readonly IList<Controller> _modules;

        public HttpService(string ipAddress, int port)
            : base(ipAddress, port)
        {
            _modules = new List<Controller>();
        }

        /// <summary>
        /// 注册模块
        /// </summary>
        /// <param name="module">ServiceModule</param>
        public void RegisterController(params Controller[] module)
        {
            foreach (var item in module)
            {
                _modules.Add(item);
            }
        }

        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <param name="module"></param>
        public void RemoveModule(Controller module)
        {
            this._modules.Remove(module);
        }

        public override void OnGet(HttpRequest request, HttpResponse response)
        {
            OnExecuteRoute(request, response);
        }

        public override void OnPost(HttpRequest request, HttpResponse response)
        {
            OnExecuteRoute(request, response);
        }

        public override void Dispose()
        {
            _modules.Clear();
        }


        private void OnExecuteRoute(HttpRequest request, HttpResponse response)
        {
            var route = ServiceRoute.Parse(request);
            var module = _modules.FirstOrDefault(m => m.SearchRoute(route));

            if (module == null)
            {
                response.Error("地址不存在").Send();
                return;
            }

            var result = module.ExecuteRoute(route, request);

            if (result is IJsonResult)
            {
                response.FromJson(result).Send();
            }
            else if (result is ActionResult actionResult)
            {
                response.FromText(actionResult.Data.ToString()).Send();
            }
        }
    }
}