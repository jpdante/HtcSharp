namespace HtcSharp.HttpModule.Mvc {
    public class HttpGetAttribute : HttpMethodAttribute {

        public HttpGetAttribute(string path) : base("GET", path) { }

    }
}
