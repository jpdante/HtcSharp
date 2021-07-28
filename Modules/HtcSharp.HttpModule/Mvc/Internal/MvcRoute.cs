namespace HtcSharp.HttpModule.Mvc.Internal {
    public class MvcRoute {
        
        public string Route { get; }

        public HttpMethodAttribute HttpMethod { get; }

        public bool RequireSession { get; }

        public bool RequireContentType { get; }

        public string ContentType { get; }

        public MvcRoute(string route) {
            Route = route;
        }

    }
}