using System.Threading.Tasks;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Abstractions {
    public interface IDirective {

        public Task Invoke(HtcHttpContext httpContext);

    }
}