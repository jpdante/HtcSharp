using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Abstractions.Routing {
    public interface IRouter {

        public Task<bool> Route(HtcHttpContext httpContext);

        public Task<IRoute> GetRoute(HtcHttpContext httpContext);

    }
}