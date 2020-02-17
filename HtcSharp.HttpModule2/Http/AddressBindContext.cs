// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using KestrelServerOptions = HtcSharp.HttpModule2.Server.KestrelServerOptions;
using ListenOptions = HtcSharp.HttpModule2.Server.ListenOptions;

namespace HtcSharp.HttpModule2.Http
{
    internal class AddressBindContext
    {
        public ICollection<string> Addresses { get; set; }
        public List<ListenOptions> ListenOptions { get; set; }
        public KestrelServerOptions ServerOptions { get; set; }
        public ILogger Logger { get; set; }

        public Func<ListenOptions, Task> CreateBinding { get; set; }
    }
}
