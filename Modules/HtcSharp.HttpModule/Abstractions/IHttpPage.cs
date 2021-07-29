using System.Threading.Tasks;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Abstractions {
    public interface IHttpPage {
        
        Task OnHttpPageRequest(DirectiveDelegate next, HtcHttpContext httpContext);

    }
}