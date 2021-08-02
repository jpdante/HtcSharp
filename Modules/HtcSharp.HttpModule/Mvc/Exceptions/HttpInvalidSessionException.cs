namespace HtcSharp.HttpModule.Mvc.Exceptions {
    public class HttpInvalidSessionException : HttpException {

        public HttpInvalidSessionException() : base(403, "Invalid session.") {

        }
    }
}