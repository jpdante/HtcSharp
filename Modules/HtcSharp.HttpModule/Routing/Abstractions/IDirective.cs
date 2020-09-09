using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule.Routing.Abstractions {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    public interface IDirective {
        Task Execute(HttpContext context);
    }
}