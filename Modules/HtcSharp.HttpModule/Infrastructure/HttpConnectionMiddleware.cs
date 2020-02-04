using System.Net;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Core;
using HtcSharp.HttpModule.Http.Http.Abstractions;
using HtcSharp.HttpModule.Infrastructure.Features;
using HtcSharp.HttpModule.Infrastructure.Interfaces;

namespace HtcSharp.HttpModule.Infrastructure {
    internal class HttpConnectionMiddleware<TContext> {
        private readonly ServiceContext _serviceContext;
        private readonly IHttpApplication<TContext> _application;
        private readonly HttpProtocols _protocols;

        public HttpConnectionMiddleware(ServiceContext serviceContext, IHttpApplication<TContext> application, HttpProtocols protocols) {
            _serviceContext = serviceContext;
            _application = application;
            _protocols = protocols;
        }

        public Task OnConnectionAsync(ConnectionContext connectionContext) {
            var memoryPoolFeature = connectionContext.Features.Get<IMemoryPoolFeature>();

            var httpConnectionContext = new HttpConnectionContext {
                ConnectionId = connectionContext.ConnectionId,
                ConnectionContext = connectionContext,
                Protocols = _protocols,
                ServiceContext = _serviceContext,
                ConnectionFeatures = connectionContext.Features,
                MemoryPool = memoryPoolFeature.MemoryPool,
                Transport = connectionContext.Transport,
                LocalEndPoint = connectionContext.LocalEndPoint as IPEndPoint,
                RemoteEndPoint = connectionContext.RemoteEndPoint as IPEndPoint
            };

            var connection = new HttpConnection(httpConnectionContext);

            return connection.ProcessRequestsAsync(_application);
        }
    }
}