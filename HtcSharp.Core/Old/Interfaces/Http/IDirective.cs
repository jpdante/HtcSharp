using HtcSharp.Core.Old.Models.Http;

namespace HtcSharp.Core.Old.Interfaces.Http {
    public interface IDirective {

        void Execute(HtcHttpContext context);

    }
}
