using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule.Routing.Abstractions {
    public interface IPageMessage {
        int StatusCode { get; }
        string GetPageMessage(HttpContext httpContext);
        Task ExecutePageMessage(HttpContext httpContext);
    }
}
