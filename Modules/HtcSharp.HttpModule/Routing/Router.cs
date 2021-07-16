using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions.Routing;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Routing {
    public class Router : IRouter {

        private readonly List<IRoute> _routes;

        public Router(List<IRoute> routes) {
            _routes = routes;
        }

        public async Task<bool> Route(HtcHttpContext httpContext) {
            var route = await GetRoute(httpContext);
            if (route == null) return false;
            await route.Route(httpContext);
            return true;
        }

        public async Task<IRoute> GetRoute(HtcHttpContext httpContext) {
            foreach (var route in _routes) {
                if (await route.Match(httpContext)) return route;
            }
            return null;
        }
    }
}