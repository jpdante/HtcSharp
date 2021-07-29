using System.Threading.Tasks;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Abstractions {
    public interface IExtensionProcessor {
        
        Task OnHttpExtensionProcess(DirectiveDelegate next, HtcHttpContext httpContext, string extension);

    }
}