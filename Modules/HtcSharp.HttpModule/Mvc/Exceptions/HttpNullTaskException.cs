namespace HtcSharp.HttpModule.Mvc.Exceptions {
    public class HttpNullTaskException : HttpException {

        public HttpNullTaskException() : base(500, "Mvc function returned a null task.") {

        }
    }
}