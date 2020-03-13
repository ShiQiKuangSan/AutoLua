namespace HttpServer.Modules
{
    public class JsonError : ActionResult, IJsonResult
    {
        internal JsonError(object result, string message = "") : base(result, false, message)
        {
        }
    }
}
