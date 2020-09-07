// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using HtcSharp.HttpModule.Server.Abstractions.Features;

namespace HtcSharp.HttpModule.Core.Internal {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\ServerAddressesFeature.cs
    // Start-At-Remote-Line 8
    // SourceTools-End
    internal class ServerAddressesFeature : IServerAddressesFeature {
        public ICollection<string> Addresses { get; } = new List<string>();
        public bool PreferHostingUrls { get; set; }
    }
}