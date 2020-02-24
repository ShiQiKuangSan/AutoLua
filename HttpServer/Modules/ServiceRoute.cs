using System;

namespace HttpServer.Modules
{
    public class ServiceRoute
    {
        public RouteMethod Method { get; private set; }
        public string RoutePath { get; private set; }

        public static ServiceRoute Parse(HttpRequest request)
        {
            var route = new ServiceRoute
            {
                Method = (RouteMethod)Enum.Parse(typeof(RouteMethod), request.Method),
                RoutePath = request.URL
            };
            return route;
        }
    }
}
