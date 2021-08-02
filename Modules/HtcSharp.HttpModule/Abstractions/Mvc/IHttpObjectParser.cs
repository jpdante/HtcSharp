using System;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Abstractions.Mvc {
    public interface IHttpObjectParser {

        public Task<IHttpObject> Parse(HtcHttpContext httpContext, Type objectType);

    }
}