using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using HtcSharp.HttpModule2.Connection.Address;

namespace HtcSharp.HttpModule2.Connection.ListenOptions {
    internal sealed class LocalhostListenOptions : ListenOptions {
        internal LocalhostListenOptions(int port) : base(new IPEndPoint(IPAddress.Loopback, port)) {
            if (port == 0) throw new InvalidOperationException("Dynamic port binding is not supported when binding to localhost. You must either bind to 127.0.0.1:0 or [::1]:0, or both.");
        }

        internal override string GetDisplayName() {
            return $"{Scheme}://localhost:{IPEndPoint.Port}";
        }

        internal override async Task BindAsync(AddressBindContext context) {
            var exceptions = new List<Exception>();

            try {
                var v4Options = Clone(IPAddress.Loopback);
                await AddressBinder.BindEndpointAsync(v4Options, context).ConfigureAwait(false);
            } catch (Exception ex) when (!(ex is IOException)) {
                context.Logger.Warn($"Unable to bind to {GetDisplayName()} on the IPv4 loopback interface: '{ex.Message}'.", ex);
                exceptions.Add(ex);
            }

            try {
                var v6Options = Clone(IPAddress.IPv6Loopback);
                await AddressBinder.BindEndpointAsync(v6Options, context).ConfigureAwait(false);
            } catch (Exception ex) when (!(ex is IOException)) {
                context.Logger.Warn($"Unable to bind to {GetDisplayName()} on the IPv6 loopback interface: '{ex.Message}'.", ex);
                exceptions.Add(ex);
            }

            if (exceptions.Count == 2) throw new IOException($"Unable to bind to {GetDisplayName()} on the IPv4 and IPv6 loopback interfaces.", new AggregateException(exceptions));
            context.Addresses.Add(GetDisplayName());
        }

        internal ListenOptions Clone(IPAddress address) {
            var options = new ListenOptions(new IPEndPoint(address, IPEndPoint.Port)) {
                HtcServerOptions = HtcServerOptions,
                Protocols = Protocols,
                IsTls = IsTls
            };

            options.Middleware.AddRange(Middleware);
            return options;
        }
    }
}
