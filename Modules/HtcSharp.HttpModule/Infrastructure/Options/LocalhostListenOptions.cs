﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Infrastructure.Binding;
using Microsoft.Extensions.Logging;

namespace HtcSharp.HttpModule.Infrastructure.Options {
    internal sealed class LocalhostListenOptions : ListenOptions {
        internal LocalhostListenOptions(int port)
            : base(new IPEndPoint(IPAddress.Loopback, port)) {
            if (port == 0) {
                throw new InvalidOperationException("Dynamic port binding is not supported when binding to localhost. You must either bind to 127.0.0.1:0 or [::1]:0, or both.");
            }
        }

        /// <summary>
        /// Gets the name of this endpoint to display on command-line when the web server starts.
        /// </summary>
        internal override string GetDisplayName() {
            return $"{Scheme}://localhost:{IPEndPoint.Port}";
        }

        internal override async Task BindAsync(AddressBindContext context) {
            var exceptions = new List<Exception>();

            try {
                var v4Options = Clone(IPAddress.Loopback);
                await AddressBinder.BindEndpointAsync(v4Options, context).ConfigureAwait(false);
            } catch (Exception ex) when (!(ex is IOException)) {
                context.Logger.LogWarning(0, "Unable to bind to {address} on the {interfaceName} interface: '{error}'.", GetDisplayName(), "IPv4 loopback", ex.Message);
                exceptions.Add(ex);
            }

            try {
                var v6Options = Clone(IPAddress.IPv6Loopback);
                await AddressBinder.BindEndpointAsync(v6Options, context).ConfigureAwait(false);
            } catch (Exception ex) when (!(ex is IOException)) {
                context.Logger.LogWarning(0, "Unable to bind to {address} on the {interfaceName} interface: '{error}'.", GetDisplayName(), "IPv6 loopback", ex.Message);
                exceptions.Add(ex);
            }

            if (exceptions.Count == 2) {
                throw new IOException($"Failed to bind to address {GetDisplayName()}.", new AggregateException(exceptions));
            }

            // If StartLocalhost doesn't throw, there is at least one listener.
            // The port cannot change for "localhost".
            context.Addresses.Add(GetDisplayName());
        }

        // used for cloning to two IPEndpoints
        internal ListenOptions Clone(IPAddress address) {
            var options = new ListenOptions(new IPEndPoint(address, IPEndPoint.Port)) {
                KestrelServerOptions = KestrelServerOptions,
                Protocols = Protocols,
                IsTls = IsTls
            };

            options._middleware.AddRange(_middleware);
            return options;
        }
    }
}