using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Routing;

namespace HtcSharp.HttpModule.Mvc {
    public class HttpHeadAttribute : HttpMethodAttribute {

        public HttpHeadAttribute(string path, bool requireSession = false) : base("HEAD", path, ContentType.DEFAULT, requireSession) { }

    }
}
