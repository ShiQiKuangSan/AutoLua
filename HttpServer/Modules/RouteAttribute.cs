using System;

namespace HttpServer.Modules
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        public RouteAttribute(RouteMethod method, string routePath)
        {
            Method = method;
            RoutePath = routePath;
        }

        public RouteMethod Method { get; set; }
        public string RoutePath { get; set; }
    }
}
