// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;

namespace HtcSharp.HttpModule2.Http.Protocols.Http
{
    internal sealed class Http1Connection<TContext> : Http1Connection, IHostContextContainer<TContext>
    {
        public Http1Connection(HttpConnectionContext context) : base(context) { }

        TContext IHostContextContainer<TContext>.HostContext { get; set; }
    }
}
