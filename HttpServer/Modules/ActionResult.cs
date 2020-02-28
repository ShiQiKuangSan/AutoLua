namespace HttpServer.Modules
{
    public class ActionResult
    {
        public ActionResult(string message, object result, bool success = true)
        {
            Result = result;
            Success = success;
            Message = message;
        }

        public object Result { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
