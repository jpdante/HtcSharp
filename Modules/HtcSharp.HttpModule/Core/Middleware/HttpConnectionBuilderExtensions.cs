// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using HtcSharp.HttpModule.Connections.Abstractions;
using HtcSharp.HttpModule.Core.Internal;
using HtcSharp.HttpModule.Server.Abstractions;

namespace HtcSharp.HttpModule.Core.Middleware {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Middleware\HttpConnectionBuilderExtensions.cs
    // Start-At-Remote-Line 10
    // SourceTools-End
    internal static class HttpConnectionBuilderExtensions {
        public static IConnectionBuilder UseHttpServer<TContext>(this IConnectionBuilder builder, ServiceContext serviceContext, IHttpApplication<TContext> application, HttpProtocols protocols) {
            var middleware = new HttpConnectionMiddleware<TContext>(serviceContext, application, protocols);
            return builder.Use(next => { return middleware.OnConnectionAsync; });
        }
    }
}