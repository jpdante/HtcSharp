namespace HtcSharp.Core.Old.Models.Http.Pages {
    public interface IPageMessage {
        int StatusCode { get; }
        string GetPageMessage(HtcHttpContext httpContext);
        void ExecutePageMessage(HtcHttpContext httpContext);
    }
}
