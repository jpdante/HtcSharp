// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Connections.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Features;
using HtcSharp.HttpModule.Infrastructure;
using HtcSharp.HttpModule.Server.Abstractions;
using Microsoft.Extensions.Logging;

namespace HtcSharp.HttpModule.Middleware {
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

            if(memoryPoolFeature == null) _serviceContext.Log.LogError("Memory pool is null, there is something wrong here!!!");

            _serviceContext.Log.LogError("Connection received, decoding request...");

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

            _serviceContext.Log.LogError("Request decoded!");

            var connection = new HttpConnection(httpConnectionContext);

            return connection.ProcessRequestsAsync(_application);
        }
    }
}
