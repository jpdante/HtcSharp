// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using HtcSharp.HttpModule.Server.Abstractions;

namespace HtcSharp.HttpModule.Core.Internal.Http2 {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http2\Http2StreamOfT.cs
    // Start-At-Remote-Line 8
    // SourceTools-End
    internal sealed class Http2Stream<TContext> : Http2Stream, IHostContextContainer<TContext> {
        private readonly IHttpApplication<TContext> _application;

        public Http2Stream(IHttpApplication<TContext> application, Http2StreamContext context) : base(context) {
            _application = application;
        }

        public override void Execute() {
            // REVIEW: Should we store this in a field for easy debugging?
            _ = ProcessRequestsAsync(_application);
        }

        // Pooled Host context
        TContext IHostContextContainer<TContext>.HostContext { get; set; }
    }
}