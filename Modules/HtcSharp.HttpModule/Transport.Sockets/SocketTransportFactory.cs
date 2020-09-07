// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Connections.Abstractions;
using HtcSharp.HttpModule.Transport.Sockets.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HtcSharp.HttpModule.Transport.Sockets {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Transport.Sockets\src\SocketTransportFactory.cs
    // Start-At-Remote-Line 15
    // SourceTools-End
    public sealed class SocketTransportFactory : IConnectionListenerFactory {
        private readonly SocketTransportOptions _options;
        private readonly SocketsTrace _trace;

        public SocketTransportFactory(
            IOptions<SocketTransportOptions> options,
            ILoggerFactory loggerFactory) {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            if (loggerFactory == null) {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _options = options.Value;
            var logger = loggerFactory.CreateLogger("Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets");
            _trace = new SocketsTrace(logger);
        }

        public ValueTask<IConnectionListener> BindAsync(EndPoint endpoint, CancellationToken cancellationToken = default) {
            var transport = new SocketConnectionListener(endpoint, _options, _trace);
            transport.Bind();
            return new ValueTask<IConnectionListener>(transport);
        }
    }
}