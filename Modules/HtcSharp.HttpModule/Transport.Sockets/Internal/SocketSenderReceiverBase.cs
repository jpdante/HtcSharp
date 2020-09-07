// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO.Pipelines;

namespace HtcSharp.HttpModule.Transport.Sockets.Internal {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Transport.Sockets\src\Internal\SocketSenderReceiverBase.cs
    // Start-At-Remote-Line 8
    // SourceTools-End
    internal abstract class SocketSenderReceiverBase : IDisposable {
        protected readonly System.Net.Sockets.Socket _socket;
        protected readonly SocketAwaitableEventArgs _awaitableEventArgs;

        protected SocketSenderReceiverBase(System.Net.Sockets.Socket socket, PipeScheduler scheduler) {
            _socket = socket;
            _awaitableEventArgs = new SocketAwaitableEventArgs(scheduler);
        }

        public void Dispose() => _awaitableEventArgs.Dispose();
    }
}