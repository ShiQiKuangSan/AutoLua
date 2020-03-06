namespace HttpServer.Modules
{
    public class JsonSuccess : ActionResult, IJsonResult
    {
        public JsonSuccess(object result, string message = "") : base(result, true, message)
        {
        }
    }
}
