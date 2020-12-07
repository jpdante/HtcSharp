using HtcSharp.HttpModule.Routing;

namespace HtcSharp.HttpModule.Mvc {
    public class HttpConnectAttribute : HttpMethodAttribute {

        public HttpConnectAttribute(string path, ContentType contentType = ContentType.DEFAULT, bool requireSession = false) : base("CONNECT", path, contentType, requireSession) { }

    }
}