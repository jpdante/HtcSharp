// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using KestrelServerOptions = HtcSharp.HttpModule2.Server.KestrelServerOptions;

namespace HtcSharp.HttpModule2.Http
{
    internal class KestrelServerOptionsSetup : IConfigureOptions<KestrelServerOptions>
    {
        private IServiceProvider _services;

        public KestrelServerOptionsSetup(IServiceProvider services)
        {
            _services = services;
        }

        public void Configure(KestrelServerOptions options)
        {
            options.ApplicationServices = _services;
        }
    }
}