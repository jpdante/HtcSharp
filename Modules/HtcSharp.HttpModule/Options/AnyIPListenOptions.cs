// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Logging;
using Microsoft.Extensions.Logging;

namespace HtcSharp.HttpModule.Options {
    internal sealed class AnyIPListenOptions : ListenOptions {
        internal AnyIPListenOptions(int port)
            : base(new IPEndPoint(IPAddress.IPv6Any, port)) {
        }

        internal override async Task BindAsync(AddressBindContext context) {
            // when address is 'http://hostname:port', 'http://*:port', or 'http://+:port'
            try {
                await base.BindAsync(context).ConfigureAwait(false);
            } catch (Exception ex) when (!(ex is IOException)) {
                context.Logger.LogDebug($@"Failed to bind to http://[::]:{IPEndPoint.Port} (IPv6Any). Attempting to bind to http://0.0.0.0:{IPEndPoint.Port} instead.");

                // for machines that do not support IPv6
                EndPoint = new IPEndPoint(IPAddress.Any, IPEndPoint.Port);
                await base.BindAsync(context).ConfigureAwait(false);
            }
        }
    }
}
