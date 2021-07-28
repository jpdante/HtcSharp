namespace HtcSharp.HttpModule.Mvc {
    public class HttpPostAttribute : HttpMethodAttribute {

        public HttpPostAttribute(string path) : base("POST", path) { }

    }
}