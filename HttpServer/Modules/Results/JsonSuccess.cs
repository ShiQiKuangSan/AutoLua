namespace HttpServer.Modules.Results
{
    public class JsonSuccess : ActionResult
    {
        public JsonSuccess(string message, object result, bool success = true) : base(message, result, success)
        {
        }
    }
}
