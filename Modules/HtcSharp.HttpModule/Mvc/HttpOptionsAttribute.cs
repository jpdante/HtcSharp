using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Routing;

namespace HtcSharp.HttpModule.Mvc {
    public class HttpOptionsAttribute : HttpMethodAttribute {

        public HttpOptionsAttribute(string path, ContentType contentType = ContentType.DEFAULT, bool requireSession = false) : base("OPTIONS", path, contentType, requireSession) { }

    }
}