using HtcSharp.HttpModule.Routing;

namespace HtcSharp.HttpModule.Mvc {
    public class HttpCustomAttribute : HttpMethodAttribute {

        public HttpCustomAttribute(string method, string path, ContentType contentType = ContentType.DEFAULT, bool requireSession = false) : base(method, path, contentType, requireSession) { }

    }
}