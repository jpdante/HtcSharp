using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule {
    public interface IHttpEvents {
        Task OnHttpPageRequest(HttpContext httpContext, string filename);
        Task OnHttpExtensionRequest(HttpContext httpContext, string filename, string extension);
    }
}
