using System;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Infrastructure.Http.Abstractions;
using HtcSharp.HttpModule.Infrastructure.Protocol;

namespace HtcSharp.HttpModule.Infrastructure.Interface {
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