// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using HtcSharp.HttpModule.Server.Abstractions;

namespace HtcSharp.HttpModule.Core.Internal.Http {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http\Http1ConnectionOfT.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    internal sealed class Http1Connection<TContext> : Http1Connection, IHostContextContainer<TContext> {
        public Http1Connection(HttpConnectionContext context) : base(context) {
        }

        TContext IHostContextContainer<TContext>.HostContext { get; set; }
    }
}