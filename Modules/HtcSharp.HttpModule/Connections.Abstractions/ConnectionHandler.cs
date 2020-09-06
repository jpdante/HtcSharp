// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Connections.Abstractions {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Connections.Abstractions\src\ConnectionHandler.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    /// <summary>
    /// Represents an end point that multiple connections connect to. For HTTP, endpoints are URLs, for non HTTP it can be a TCP listener (or similar)
    /// </summary>
    public abstract class ConnectionHandler {
        /// <summary>
        /// Called when a new connection is accepted to the endpoint
        /// </summary>
        /// <param name="connection">The new <see cref="ConnectionContext"/></param>
        /// <returns>A <see cref="Task"/> that represents the connection lifetime. When the task completes, the connection is complete.</returns>
        public abstract Task OnConnectedAsync(ConnectionContext connection);
    }
}