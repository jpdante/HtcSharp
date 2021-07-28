namespace HtcSharp.HttpModule.Mvc {
    public class HttpOptionsAttribute : HttpMethodAttribute {

        public HttpOptionsAttribute(string path) : base("OPTIONS", path) { }

    }
}