namespace HtcSharp.HttpModule.Mvc {
    public class HttpTraceAttribute : HttpMethodAttribute {

        public HttpTraceAttribute(string path) : base("TRACE", path) { }

    }
}