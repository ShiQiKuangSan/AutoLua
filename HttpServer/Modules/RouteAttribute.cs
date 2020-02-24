using System;

namespace HttpServer.Modules
{
    [AttributeUsage(AttributeTargets.Method)]
    class RouteAttribute : Attribute
    {
        public RouteMethod Method { get; set; }
        public string RoutePath { get; set; }
    }
}
