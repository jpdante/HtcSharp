using System;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Core.Http;
using HtcSharp.HttpModule.Http.Http.Abstractions;

namespace HtcSharp.HttpModule.Core {
    internal interface IRequestProcessor {
        Task ProcessRequestsAsync<TContext>(IHttpApplication<TContext> application);
        void StopProcessingNextRequest();
        void HandleRequestHeadersTimeout();
        void HandleReadDataRateTimeout();
        void OnInputOrOutputCompleted();
        void Tick(DateTimeOffset now);
        void Abort(ConnectionAbortedException ex);
    }
}