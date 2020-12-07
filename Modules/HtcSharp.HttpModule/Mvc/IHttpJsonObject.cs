using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule.Mvc {
    public interface IHttpJsonObject {

        public Task<bool> ValidateData(HttpContext httpContext);

    }
}