// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Features;

namespace HtcSharp.HttpModule.Server.Abstractions {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Hosting\Server.Abstractions\src\IServer.cs
    // Start-At-Remote-Line 10
    // SourceTools-End
    /// <summary>
    /// Represents a server.
    /// </summary>
    public interface IServer : IDisposable {
        /// <summary>
        /// A collection of HTTP features of the server.
        /// </summary>
        IFeatureCollection Features { get; }

        /// <summary>
        /// Start the server with an application.
        /// </summary>
        /// <param name="application">An instance of <see cref="IHttpApplication{TContext}"/>.</param>
        /// <typeparam name="TContext">The context associated with the application.</typeparam>
        /// <param name="cancellationToken">Indicates if the server startup should be aborted.</param>
        Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken);

        /// <summary>
        /// Stop processing requests and shut down the server, gracefully if possible.
        /// </summary>
        /// <param name="cancellationToken">Indicates if the graceful shutdown should be aborted.</param>
        Task StopAsync(CancellationToken cancellationToken);
    }
}