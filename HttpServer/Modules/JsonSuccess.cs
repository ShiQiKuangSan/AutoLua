namespace HttpServer.Modules
{
    public class JsonSuccess : ActionResult, IJsonResult
    {
        internal JsonSuccess(object result, string message = "") : base(result, true, message)
        {
        }
    }
}
