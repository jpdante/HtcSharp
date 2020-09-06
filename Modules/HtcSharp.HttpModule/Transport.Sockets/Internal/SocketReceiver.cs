// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO.Pipelines;

namespace HtcSharp.HttpModule.Transport.Sockets.Internal {
    internal sealed class SocketReceiver : SocketSenderReceiverBase {
        public SocketReceiver(System.Net.Sockets.Socket socket, PipeScheduler scheduler) : base(socket, scheduler) {
        }

        public SocketAwaitableEventArgs WaitForDataAsync() {
            _awaitableEventArgs.SetBuffer(Memory<byte>.Empty);

            if (!_socket.ReceiveAsync(_awaitableEventArgs)) {
                _awaitableEventArgs.Complete();
            }

            return _awaitableEventArgs;
        }

        public SocketAwaitableEventArgs ReceiveAsync(Memory<byte> buffer) {
            _awaitableEventArgs.SetBuffer(buffer);

            if (!_socket.ReceiveAsync(_awaitableEventArgs)) {
                _awaitableEventArgs.Complete();
            }

            return _awaitableEventArgs;
        }
    }
}