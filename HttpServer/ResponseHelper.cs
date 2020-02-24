namespace HttpServer
{
    public static class ResponseHelper
    {
        public static HttpResponse FromXML(this HttpResponse response, string xmlText)
        {
            response.SetContent(xmlText);
            response.Content_Type = "text/xml";
            response.StatusCode = "200";
            return response;
        }

        public static HttpResponse FromXML<T>(this HttpResponse response, T entity) where T : class
        {
            return response.FromXML("");
        }

        public static HttpResponse FromJSON(this HttpResponse response, string jsonText)
        {
            response.SetContent(jsonText);
            response.Content_Type = "text/json";
            response.StatusCode = "200";
            return response;
        }

        public static HttpResponse FromJSON<T>(this HttpResponse response, T entity) where T : class
        {
            return response.FromJSON("");
        }

        public static HttpResponse FromText(this HttpResponse response, string text)
        {
            response.SetContent(text);
            response.Content_Type = "text/plain";
            response.StatusCode = "200";
            return response;
        }
    }
}
