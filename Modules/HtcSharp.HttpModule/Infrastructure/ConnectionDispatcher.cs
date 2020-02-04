﻿using System;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Core.Infrastructure;
using HtcSharp.HttpModule.Http.Http.Abstractions;
using HtcSharp.HttpModule.Infrastructure.Interfaces;

namespace HtcSharp.HttpModule.Infrastructure {
    internal class ConnectionDispatcher {
        private static long _lastConnectionId = long.MinValue;

        private readonly ServiceContext _serviceContext;
        private readonly ConnectionDelegate _connectionDelegate;
        private readonly TaskCompletionSource<object> _acceptLoopTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

        public ConnectionDispatcher(ServiceContext serviceContext, ConnectionDelegate connectionDelegate) {
            _serviceContext = serviceContext;
            _connectionDelegate = connectionDelegate;
        }

        private IKestrelTrace Log => _serviceContext.Log;

        public Task StartAcceptingConnections(IConnectionListener listener) {
            ThreadPool.UnsafeQueueUserWorkItem(StartAcceptingConnectionsCore, listener, preferLocal: false);
            return _acceptLoopTcs.Task;
        }

        private void StartAcceptingConnectionsCore(IConnectionListener listener) {
            // REVIEW: Multiple accept loops in parallel?
            _ = AcceptConnectionsAsync();

            async Task AcceptConnectionsAsync() {
                try {
                    while (true) {
                        var connection = await listener.AcceptAsync();

                        if (connection == null) {
                            // We're done listening
                            break;
                        }

                        // Add the connection to the connection manager before we queue it for execution
                        var id = Interlocked.Increment(ref _lastConnectionId);
                        var kestrelConnection = new KestrelConnection(id, _serviceContext, _connectionDelegate, connection, Log);

                        _serviceContext.ConnectionManager.AddConnection(id, kestrelConnection);

                        Log.ConnectionAccepted(connection.ConnectionId);

                        ThreadPool.UnsafeQueueUserWorkItem(kestrelConnection, preferLocal: false);
                    }
                } catch (Exception ex) {
                    // REVIEW: If the accept loop ends should this trigger a server shutdown? It will manifest as a hang
                    Log.LogCritical(0, ex, "The connection listener failed to accept any new connections.");
                } finally {
                    _acceptLoopTcs.TrySetResult(null);
                }
            }
        }
    }
}
