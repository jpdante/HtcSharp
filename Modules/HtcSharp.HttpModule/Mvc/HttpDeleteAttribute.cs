using HtcSharp.HttpModule.Routing;

namespace HtcSharp.HttpModule.Mvc {
    public class HttpDeleteAttribute : HttpMethodAttribute {

        public HttpDeleteAttribute(string path, ContentType contentType = ContentType.DEFAULT, bool requireSession = false) : base("DELETE", path, contentType, requireSession) { }

    }
}