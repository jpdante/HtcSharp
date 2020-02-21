using HtcSharp.Core.Old.Models.Http;

namespace HtcSharp.Core.Old.Interfaces.Plugin {
    public interface IHttpEvents {
        bool OnHttpPageRequest(HtcHttpContext httpContext, string filename);
        bool OnHttpExtensionRequest(HtcHttpContext httpContext, string filename, string extension);
    }
}
