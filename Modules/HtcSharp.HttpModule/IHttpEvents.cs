using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    public interface IHttpEvents {
        Task OnHttpPageRequest(HttpContext httpContext, string filename);
        Task OnHttpExtensionRequest(HttpContext httpContext, string filename, string extension);
    }
}