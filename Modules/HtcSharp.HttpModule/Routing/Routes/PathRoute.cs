using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions.Routing;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Routing.Routes {
    public class PathRoute : IRoute {

        private readonly Regex _regex;

        public PathRoute(string pattern) {
            _regex = new Regex(pattern);
        }

        public Task<bool> Match(HtcHttpContext httpContext) {
            return Task.FromResult(_regex.IsMatch(httpContext.Request.Path.ToUriComponent()));
        }

        public Task Route(HtcHttpContext httpContext) {
            throw new System.NotImplementedException();
        }

    }
}