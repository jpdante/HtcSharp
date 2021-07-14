using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Abstractions {
    public interface IHttpEvents {
        
        Task OnHttpPageRequest(HtcHttpContext httpContext, string filename);

        Task OnHttpExtensionRequest(HtcHttpContext httpContext, string filename, string extension);

    }
}