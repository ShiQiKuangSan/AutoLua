using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HttpServer.Modules
{
    public abstract class Controller
    {
        private readonly IEnumerable<RouteAttribute> _routs;
        private readonly IEnumerable<MethodInfo> _methods;

        protected Controller()
        {
            var type = GetType();
            _methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<RouteAttribute>(true) != null)
                .ToArray();

            _routs = _methods.Select(x => x.GetCustomAttribute<RouteAttribute>()).ToArray();
        }

        internal bool SearchRoute(ServiceRoute route)
        {
            var status = false;

            foreach (var item in _routs)
            {
                status = item.Method == route.Method && item.RoutePath == route.RoutePath;
                if (status)
                    break;
            }

            return status;
        }

        internal object ExecuteRoute(ServiceRoute route, HttpRequest request)
        {
            if (!_methods.Any()) return null;

            var method = _methods.FirstOrDefault(m =>
            {
                var attribute = m.GetCustomAttribute<RouteAttribute>(true);

                if (attribute == null)
                    return false;

                if (request.Params.Any())
                {
                    return attribute.Method == route.Method && attribute.RoutePath == route.RoutePath &&
                           m.GetParameters().Length > 0;
                }
                else
                {
                    return attribute.Method == route.Method && attribute.RoutePath == route.RoutePath;
                }
            });

            if (method == null)
                return null;

            //方法的参数集合
            var parameterInfos = method.GetParameters();

            //解析后的参数集合
            var list = new List<object>();

            if (parameterInfos.Any() && request.Params.Any())
            {
                switch (request.Method)
                {
                    case "GET":
                        ResolveGet(parameterInfos, request, list);
                        break;
                    case "POST":
                        ResolvePost(parameterInfos, request, list);
                        break;
                }
            }

            try
            {
                if (parameterInfos.Length != list.Count)
                {
                    return JsonError(null, "参数获取失败");
                }

                return method.Invoke(this, list.ToArray());
            }
            catch (Exception e)
            {
                return JsonError(e.Message);
            }
        }

        /// <summary>
        /// 解析get参数。
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="request"></param>
        /// <param name="methods"></param>
        private static void ResolveGet(IEnumerable<ParameterInfo> infos, HttpRequest request,
            ICollection<object> methods)
        {
            var dis = request.Params.ToDictionary(item => item.Key.ToUpper(), item => item.Value);

            foreach (var info in infos)
            {
                var isObject = IsObject(info);
                //不是对象的话，可以进行转换。
                if (!isObject)
                {
                    //判断参数全大小模式下是否匹配
                    var containsKey = dis.ContainsKey(info.Name.ToUpper());

                    //第一个参数名称，不通过
                    if (!containsKey)
                        break;

                    var value = request.Params[info.Name];

                    var newValue = GetType(info.ParameterType, value);

                    methods.Add(newValue ?? value);
                }
            }
        }

        /// <summary>
        /// 解析post参数。
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="request"></param>
        /// <param name="methods"></param>
        private static void ResolvePost(IEnumerable<ParameterInfo> infos, HttpRequest request,
            ICollection<object> methods)
        {
            try
            {
                var dis = request.Params
                    .ToDictionary(item => item.Key.ToUpper(), item => item.Value);

                foreach (var info in infos)
                {
                    var isObject = IsObject(info);

                    if (isObject)
                    {
                        //实例化对象
                        var obj = CreateInstance(info, dis);

                        if (obj == null)
                            continue;

                        methods.Add(obj);
                    }
                    else
                    {
                        //判断参数全大小模式下是否匹配
                        var containsKey = dis.ContainsKey(info.Name.ToUpper());

                        //第一个参数名称，不通过
                        if (!containsKey)
                            continue;

                        var value = request.Params[info.Name];

                        var newValue = GetType(info.ParameterType, value);

                        methods.Add(newValue ?? value);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// 创建参数实例
        /// </summary>
        /// <param name="info">参数信息</param>
        /// <param name="dis">url参数信息</param>
        /// <returns></returns>
        private static object CreateInstance(ParameterInfo info, IReadOnlyDictionary<string, string> dis)
        {
            //获得方法参数类型
            var infoType = info.ParameterType;

            //获得方法参数 所在程序集的类型。
            var type = infoType.Assembly.GetTypes().FirstOrDefault(x => x.FullName == infoType.FullName);

            if (type == null)
                return null;

            //实例化当前类
            var mode = Activator.CreateInstance(type);

            if (mode == null)
                return null;

            //获得实例运行时的属性。非字段
            var properties = mode.GetType().GetRuntimeProperties().ToList();

            if (!properties.Any())
                return null;

            foreach (var item in properties)
            {
                //判断url的参数中是否与 属性名对应
                if (!dis.ContainsKey(item.Name.ToUpper()))
                    continue;

                //获得url 参数的值。
                var value = dis[item.Name.ToUpper()];

                var pType = item.PropertyType;

                var newValue = GetType(pType, value);

                item.SetValue(mode, newValue);
            }

            return mode;
        }


        /// <summary>
        /// 是否是对象
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static bool IsObject(ParameterInfo info)
        {
            //获得方法参数类型
            var infoType = info.ParameterType;

            //如果是系统自带基础类型，则返回false
            if (infoType.Assembly.FullName.StartsWith("mscorlib"))
                return false;

            //获得方法参数 所在程序集的类型。
            var type = infoType.Assembly.GetTypes().FirstOrDefault(x => x.FullName == infoType.FullName);

            return type != null;
        }

        private static object GetType(Type type, string value)
        {
            if (type == typeof(bool))
            {
                var status = bool.TryParse(value, out var result);
                if (!status)
                    return null;

                return result;
            }

            if (type == typeof(int))
            {
                var status = int.TryParse(value, out var result);
                if (!status)
                    return null;

                return result;
            }

            if (type == typeof(long))
            {
                var status = long.TryParse(value, out var result);
                if (!status)
                    return null;

                return result;
            }

            if (type == typeof(float))
            {
                var status = float.TryParse(value, out var result);
                if (!status)
                    return null;

                return result;
            }

            if (type == typeof(double))
            {
                var status = double.TryParse(value, out var result);
                if (!status)
                    return null;

                return result;
            }

            return null;
        }

        protected static JsonSuccess JsonSuccess(object data = null, string message = "")
        {
            return new JsonSuccess(data, message);
        }

        protected static JsonError JsonError(object data = null, string message = "")
        {
            return new JsonError(data, message);
        }

        protected static ActionResult Html(object data)
        {
            return new ActionResult(data, true);
        }
    }
}