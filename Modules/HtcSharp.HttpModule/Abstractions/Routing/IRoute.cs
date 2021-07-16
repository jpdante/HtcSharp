using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Abstractions.Routing {
    public interface IRoute {

        public Task<bool> Match(HtcHttpContext httpContext);

        public Task Route(HtcHttpContext httpContext);

    }
}