namespace HtcSharp.HttpModule.Mvc {
    public class HttpConnectAttribute : HttpMethodAttribute {

        public HttpConnectAttribute(string path) : base("CONNECT", path) { }

    }
}