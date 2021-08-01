using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Mvc {
    public struct RouteContext {

        public string Path { get; private set; }
        public string Method { get; private set; }
        public bool RequireSession { get; private set; }
        public string RequiredContentType { get; private set; }

        public RouteContext(HttpMethodAttribute httpMethod) {
            Path = httpMethod.Path;
            Method = httpMethod.Method;
            RequireSession = false;
            RequiredContentType = null;
        }

        public void SetRequireSession(bool value) {
            RequireSession = value;
        }

        public void SetRequiredContentType(string contentType) {
            RequiredContentType = contentType;
        }

        public void ProcessRequest(HtcHttpContext httpContext) {

        }

    }
}