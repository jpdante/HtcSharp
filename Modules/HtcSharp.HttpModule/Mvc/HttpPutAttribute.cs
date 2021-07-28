using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Routing;

namespace HtcSharp.HttpModule.Mvc {
    public class HttpPutAttribute : HttpMethodAttribute {

        public HttpPutAttribute(string path, ContentType contentType = ContentType.DEFAULT, bool requireSession = false) : base("PUT", path, contentType, requireSession) { }

    }
}