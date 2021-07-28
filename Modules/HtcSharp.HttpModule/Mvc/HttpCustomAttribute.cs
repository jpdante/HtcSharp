namespace HtcSharp.HttpModule.Mvc {
    public class HttpCustomAttribute : HttpMethodAttribute {

        public HttpCustomAttribute(string method, string path) : base(method, path) { }

    }
}