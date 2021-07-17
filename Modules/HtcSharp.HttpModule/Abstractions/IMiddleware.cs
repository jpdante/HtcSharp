using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Abstractions {
    public interface IMiddleware {

        public Task Invoke(HtcHttpContext httpContext);

    }
}