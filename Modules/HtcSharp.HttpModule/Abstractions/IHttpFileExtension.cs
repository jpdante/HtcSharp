using System.Threading.Tasks;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Abstractions {
    public interface IHttpFileExtension {
        
        Task OnHttpFileExtension(DirectiveDelegate next, HtcHttpContext httpContext, string fileName, string extension);

    }
}