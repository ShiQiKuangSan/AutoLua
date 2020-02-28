using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HttpServer.Modules
{
    public class Controller
    {
        private readonly Type type;
        private readonly IEnumerable<RouteAttribute> routs = new List<RouteAttribute>();
        private readonly IEnumerable<MethodInfo> methods = new List<MethodInfo>();

        protected Controller()
        {
            type = GetType();
            methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<RouteAttribute>(true) != null)
                .ToArray();

            routs = methods.Select(x => x.GetCustomAttribute<RouteAttribute>()).ToArray();
        }

        internal bool SearchRoute(ServiceRoute route)
        {
            var status = false;

            foreach (var item in routs)
            {
                status = item.Method == route.Method && item.RoutePath == route.RoutePath;
                if (status)
                    break;
            }

            return status;
        }

        internal ActionResult ExecuteRoute(ServiceRoute route, HttpRequest request)
        {
            if (!methods.Any()) return null;

            var method = methods.FirstOrDefault(m =>
            {
                var attribute = m.GetCustomAttribute<RouteAttribute>(true);

                if (attribute == null)
                    return false;

                if (request.Params.Any())
                {
                    return attribute.Method == route.Method && attribute.RoutePath == route.RoutePath && m.GetParameters().Length > 0;
                }
                else
                {
                    return attribute.Method == route.Method && attribute.RoutePath == route.RoutePath;
                }
            });

            if (method == null)
                return null;

            var parms = method.GetParameters();

            var methodParms = new List<object>();

            if (parms.Any() && request.Params.Any())
            {
                if (request.Method == "GET")
                {
                    ResolveGet(parms, request, methodParms);
                }
                else if (request.Method == "POST")
                {
                    ResolvePost(parms, request, methodParms);
                }
            }

            try
            {
                return (ActionResult)method.Invoke(this, methodParms.ToArray());
            }
            catch (Exception e)
            {
                return new ActionResult(e.Message, null, false);
            };
        }

        /// <summary>
        /// 解析get参数。
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="request"></param>
        /// <param name="methodParms"></param>
        private static void ResolveGet(ParameterInfo[] parms, HttpRequest request, List<object> methodParms)
        {
            for (var i = 0; i < parms.Length; i++)
            {
                var parm = parms[i];

                var isParem = request.Params.ContainsKey(parm.Name);

                //第一个参数名称，不通过
                if (!isParem)
                    break;

                var reParm = request.Params[parm.Name];

                if (parm.ParameterType == typeof(bool))
                {
                    var status = bool.TryParse(reParm, out var result);
                    if (!status)
                        break;

                    methodParms.Add(result);
                }
                else if (parm.ParameterType == typeof(int))
                {
                    var status = int.TryParse(reParm, out var result);
                    if (!status)
                        break;

                    methodParms.Add(result);
                }
                else if (parm.ParameterType == typeof(long))
                {
                    var status = long.TryParse(reParm, out var result);
                    if (!status)
                        break;

                    methodParms.Add(result);
                }
                else if (parm.ParameterType == typeof(float))
                {
                    var status = float.TryParse(reParm, out var result);
                    if (!status)
                        break;

                    methodParms.Add(result);
                }
                else if (parm.ParameterType == typeof(double))
                {
                    var status = double.TryParse(reParm, out var result);
                    if (!status)
                        break;

                    methodParms.Add(result);
                }
                else
                {
                    methodParms.Add(reParm);
                }
            }
        }

        /// <summary>
        /// 解析post参数。
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="request"></param>
        /// <param name="methodParms"></param>
        private static void ResolvePost(ParameterInfo[] parms, HttpRequest request, List<object> methodParms)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request.Params);

                for (var i = 0; i < parms.Length; i++)
                {
                    var parm = parms[i];

                    var obj = JsonConvert.DeserializeObject(json, parm.ParameterType);

                    methodParms.Add(obj);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
