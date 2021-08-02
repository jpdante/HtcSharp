namespace HtcSharp.HttpModule.Mvc.Exceptions {
    public class HttpInvalidContentTypeException : HttpException {

        public HttpInvalidContentTypeException() : base(400, "Invalid content-type.") {

        }
    }
}