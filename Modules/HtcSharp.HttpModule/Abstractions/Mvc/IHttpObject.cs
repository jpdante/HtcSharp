using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Abstractions.Mvc {
    public interface IHttpObject {
        
        public Task<bool> ValidateData(HtcHttpContext httpContext);

    }
}