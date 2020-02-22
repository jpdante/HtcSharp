using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule {
    public interface IHttpEvents {
        bool OnHttpPageRequest(HttpContext httpContext, string filename);
        bool OnHttpExtensionRequest(HttpContext httpContext, string filename, string extension);
    }
}
