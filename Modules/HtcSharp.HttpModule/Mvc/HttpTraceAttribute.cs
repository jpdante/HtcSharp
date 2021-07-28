using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Routing;

namespace HtcSharp.HttpModule.Mvc {
    public class HttpTraceAttribute : HttpMethodAttribute {

        public HttpTraceAttribute(string path, ContentType contentType = ContentType.DEFAULT, bool requireSession = false) : base("TRACE", path, contentType, requireSession) { }

    }
}