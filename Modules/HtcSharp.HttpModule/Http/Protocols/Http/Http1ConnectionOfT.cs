// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using HtcSharp.HttpModule.Infrastructure;
using HtcSharp.HttpModule.Server.Abstractions;

namespace HtcSharp.HttpModule.Http.Protocols.Http
{
    internal sealed class Http1Connection<TContext> : Http1Connection, IHostContextContainer<TContext>
    {
        public Http1Connection(HttpConnectionContext context) : base(context) { }

        TContext IHostContextContainer<TContext>.HostContext { get; set; }
    }
}
