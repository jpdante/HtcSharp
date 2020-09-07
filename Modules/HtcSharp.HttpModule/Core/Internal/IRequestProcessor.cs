// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Connections.Abstractions.Exceptions;
using HtcSharp.HttpModule.Server.Abstractions;

namespace HtcSharp.HttpModule.Core.Internal {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\IRequestProcessor.cs
    // Start-At-Remote-Line 10
    // SourceTools-End
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