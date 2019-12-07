using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.HttpModule2.Connection.Exceptions;
using HtcSharp.HttpModule2.Connection.Features;

namespace HtcSharp.HttpModule2.Connection {
    public abstract class ConnectionContext : IAsyncDisposable {

        public abstract string ConnectionId { get; set; }

        public abstract IFeatureCollection Features { get; }

        public abstract IDictionary<object, object> Items { get; set; }

        public abstract IDuplexPipe Transport { get; set; }

        public virtual CancellationToken ConnectionClosed { get; set; }

        public virtual EndPoint LocalEndPoint { get; set; }

        public virtual EndPoint RemoteEndPoint { get; set; }

        public virtual void Abort(ConnectionAbortedException abortReason) {
            // We expect this to be overridden, but this helps maintain back compat
            // with implementations of ConnectionContext that predate the addition of
            // ConnectionContext.Abort()
            Features.Get<IConnectionLifetimeFeature>()?.Abort();
        }

        public virtual void Abort() => Abort(new ConnectionAbortedException("The connection was aborted by the application via ConnectionContext.Abort()."));

        public virtual ValueTask DisposeAsync() {
            return default;
        }
    }
}
