using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using HtcSharp.HttpModule2.Connection.Address;
using HtcSharp.HttpModule2.Core;

namespace HtcSharp.HttpModule2.Connection.ListenOptions {
    public class ListenOptions : IConnectionBuilder {
        internal readonly List<Func<ConnectionDelegate, ConnectionDelegate>> Middleware =
            new List<Func<ConnectionDelegate, ConnectionDelegate>>();

        internal ListenOptions(IPEndPoint endPoint) { EndPoint = endPoint; }

        internal ListenOptions(string socketPath) { EndPoint = new UnixDomainSocketEndPoint(socketPath); }

        internal ListenOptions(ulong fileHandle) : this(fileHandle, FileHandleType.AUTO) { }

        internal ListenOptions(ulong fileHandle, FileHandleType handleType) {
            EndPoint = new FileHandleEndPoint(fileHandle, handleType);
        }

        internal EndPoint EndPoint { get; set; }

        public IPEndPoint IPEndPoint => EndPoint as IPEndPoint;

        public string SocketPath => (EndPoint as UnixDomainSocketEndPoint)?.ToString();

        public ulong FileHandle => (EndPoint as FileHandleEndPoint)?.FileHandle ?? 0;

        public HtcServerOptions HtcServerOptions { get; internal set; }

        public HttpProtocols Protocols { get; set; } = HttpProtocols.HTTP1_AND_HTTP2;

        internal string Scheme {
            get {
                if (IsHttp) return IsTls ? "https" : "http";
                return "tcp";
            }
        }

        internal bool IsHttp { get; set; } = true;

        internal bool IsTls { get; set; }

        public IServiceProvider ApplicationServices => HtcServerOptions?.ApplicationServices;

        public IConnectionBuilder Use(Func<ConnectionDelegate, ConnectionDelegate> middleware) {
            Middleware.Add(middleware);
            return this;
        }

        public ConnectionDelegate Build() {
            ConnectionDelegate app = context => Task.CompletedTask;
            for (var i = Middleware.Count - 1; i >= 0; i--) {
                var component = Middleware[i];
                app = component(app);
            }

            return app;
        }

        internal virtual string GetDisplayName() {
            return EndPoint switch {
                IPEndPoint _ => $"{Scheme}://{IPEndPoint}",
                UnixDomainSocketEndPoint _ => $"{Scheme}://unix:{EndPoint}",
                FileHandleEndPoint _ => $"{Scheme}://<file handle>",
                _ => throw new InvalidOperationException()
            };
        }

        public override string ToString() { return GetDisplayName(); }

        internal virtual async Task BindAsync(AddressBindContext context) {
            await AddressBinder.BindEndpointAsync(this, context).ConfigureAwait(false);
            context.Addresses.Add(GetDisplayName());
        }
    }
}