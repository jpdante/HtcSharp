// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Connections.Abstractions {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Connections.Abstractions\src\ConnectionDelegate.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    /// <summary>
    /// A function that can process a connection.
    /// </summary>
    /// <param name="connection">A <see cref="Task" /> representing the connection.</param>
    /// <returns>A <see cref="ConnectionContext"/> that represents the connection lifetime. When the task completes, the connection will be closed.</returns>
    public delegate Task ConnectionDelegate(ConnectionContext connection);
}