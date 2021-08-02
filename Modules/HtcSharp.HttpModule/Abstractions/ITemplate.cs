using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Abstractions {
    public interface ITemplate {

        public bool SupportGetString { get; }

        public Task<string> GetString();

        public Task<string> GetReplaced(HtcHttpContext httpContext);

        public Task SendTemplate(HtcHttpContext httpContext);

    }
}