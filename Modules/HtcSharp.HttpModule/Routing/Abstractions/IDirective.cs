using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule.Routing.Abstractions {
    public interface IDirective {

        Task Execute(HttpContext context);

    }
}
