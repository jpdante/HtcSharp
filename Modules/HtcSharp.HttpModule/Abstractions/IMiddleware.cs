using System;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Abstractions {
    public interface IMiddleware {

        public Task Use(HtcHttpContext httpContext, Func<Task> next);

    }
}