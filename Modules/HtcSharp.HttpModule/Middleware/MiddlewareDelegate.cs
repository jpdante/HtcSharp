using System;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Middleware {

    public delegate Task RequestDelegate(HtcHttpContext context);

}