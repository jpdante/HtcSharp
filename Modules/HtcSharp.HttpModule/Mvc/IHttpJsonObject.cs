using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Mvc {
    public interface IHttpJsonObject {

        public Task<bool> ValidateData(HtcHttpContext httpContext);

    }
}