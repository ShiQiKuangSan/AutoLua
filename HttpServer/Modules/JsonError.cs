namespace HttpServer.Modules
{
    public class JsonError : ActionResult, IJsonResult
    {
        public JsonError(object result, string message = "") : base(result, false, message)
        {
        }
    }
}
