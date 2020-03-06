namespace HttpServer.Modules
{
    public class ActionResult
    {
        public ActionResult(object data, bool status = false, string message = "")
        {
            Data = data;
            Status = status;
            Message = message;
        }

        public object Data { get; set; }

        public bool Status { get; set; }

        public string Message { get; set; }
    }
}
