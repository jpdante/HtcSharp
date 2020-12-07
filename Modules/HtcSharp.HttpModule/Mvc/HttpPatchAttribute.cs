using HtcSharp.HttpModule.Routing;

namespace HtcSharp.HttpModule.Mvc {
    public class HttpPatchAttribute : HttpMethodAttribute {

        public HttpPatchAttribute(string path, ContentType contentType = ContentType.DEFAULT, bool requireSession = false) : base("PATCH", path, contentType, requireSession) { }

    }
}