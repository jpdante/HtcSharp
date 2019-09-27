namespace HtcSharp.HttpModule.Model.Http {
    public class HttpContext {
        public readonly HttpConnection Connection;
        public readonly HttpRequest Request;
        public readonly HttpResponse Response;

        public HttpContext(HttpConnection connection, HttpRequest request, HttpResponse response) {
            Connection = connection;
            Request = request;
            Response = response;
        }
    }
}