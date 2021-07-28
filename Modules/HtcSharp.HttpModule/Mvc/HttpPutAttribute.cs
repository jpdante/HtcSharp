namespace HtcSharp.HttpModule.Mvc {
    public class HttpPutAttribute : HttpMethodAttribute {

        public HttpPutAttribute(string path) : base("PUT", path) { }

    }
}