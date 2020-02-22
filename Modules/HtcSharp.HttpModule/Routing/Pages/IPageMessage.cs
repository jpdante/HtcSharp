using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule.Routing.Pages {
    public interface IPageMessage {
        int StatusCode { get; }
        string GetPageMessage(HttpContext httpContext);
        void ExecutePageMessage(HttpContext httpContext);
    }
}
