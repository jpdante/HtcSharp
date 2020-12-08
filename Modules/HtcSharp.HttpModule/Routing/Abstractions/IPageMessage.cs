using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule.Routing.Abstractions {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    public interface IPageMessage {
        int StatusCode { get; }
        Task<string> GetPageMessage(HttpContext httpContext);
        Task ExecutePageMessage(HttpContext httpContext);
    }
}