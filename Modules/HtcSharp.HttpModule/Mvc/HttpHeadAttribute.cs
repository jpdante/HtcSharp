namespace HtcSharp.HttpModule.Mvc {
    public class HttpHeadAttribute : HttpMethodAttribute {

        public HttpHeadAttribute(string path) : base("HEAD", path) { }

    }
}
