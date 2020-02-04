using System;

namespace HtcSharp.HttpModule.Core.Infrastructure {
    internal class ConnectionReference {
        private readonly WeakReference<KestrelConnection> _weakReference;

        public ConnectionReference(KestrelConnection connection) {
            _weakReference = new WeakReference<KestrelConnection>(connection);
            ConnectionId = connection.TransportConnection.ConnectionId;
        }

        public string ConnectionId { get; }

        public bool TryGetConnection(out KestrelConnection connection) {
            return _weakReference.TryGetTarget(out connection);
        }
    }
}
