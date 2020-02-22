using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule.Routing.Abstractions {
    public interface IDirective {

        void Execute(HttpContext context);

    }
}
