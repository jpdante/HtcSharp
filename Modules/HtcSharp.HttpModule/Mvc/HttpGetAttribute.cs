using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Routing;

namespace HtcSharp.HttpModule.Mvc {
    public class HttpGetAttribute : HttpMethodAttribute {

        public HttpGetAttribute(string path, bool requireSession = false) : base("GET", path, ContentType.DEFAULT, requireSession) { }

    }
}
