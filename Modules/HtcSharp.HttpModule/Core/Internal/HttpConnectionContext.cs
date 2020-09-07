// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using HtcSharp.HttpModule.Connections.Abstractions;
using HtcSharp.HttpModule.Core.Internal.Infrastructure;
using HtcSharp.HttpModule.Http.Features;

namespace HtcSharp.HttpModule.Core.Internal {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\HttpConnectionContext.cs
    // Start-At-Remote-Line 12
    // SourceTools-End
    internal class HttpConnectionContext {
        public string ConnectionId { get; set; }
        public HttpProtocols Protocols { get; set; }
        public ConnectionContext ConnectionContext { get; set; }
        public ServiceContext ServiceContext { get; set; }
        public IFeatureCollection ConnectionFeatures { get; set; }
        public MemoryPool<byte> MemoryPool { get; set; }
        public IPEndPoint LocalEndPoint { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
        public ITimeoutControl TimeoutControl { get; set; }
        public IDuplexPipe Transport { get; set; }
    }
}