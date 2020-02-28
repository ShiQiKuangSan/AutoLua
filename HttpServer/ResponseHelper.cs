using Newtonsoft.Json;

namespace HttpServer
{
    public static class ResponseHelper
    {
        public static HttpResponse FromXml(this HttpResponse response, string xmlText)
        {
            response.SetContent(xmlText);
            response.Content_Type = "text/xml";
            response.StatusCode = "200";
            return response;
        }

        public static HttpResponse FromXml<T>(this HttpResponse response, T entity) where T : class
        {
            return response.FromXml("");
        }

        public static HttpResponse FromJson(this HttpResponse response, string jsonText)
        {
            response.SetContent(jsonText);
            response.Content_Type = "application/json; charset=UTF-8";
            response.StatusCode = "200";
            return response;
        }

        public static HttpResponse FromJson<T>(this HttpResponse response, T entity) where T : class
        {
            var json = JsonConvert.SerializeObject(entity);
            return response.FromJson(json);
        }

        public static HttpResponse FromText(this HttpResponse response, string text)
        {
            response.SetContent(text);
            response.Content_Type = "text/html; charset=UTF-8";
            response.StatusCode = "200";
            return response;
        }

        public static HttpResponse Error(this HttpResponse response, string message)
        {
            response.SetContent(message);
            response.Content_Type = "text/html; charset=UTF-8";
            response.StatusCode = "404";
            return response;
        }
    }
}
