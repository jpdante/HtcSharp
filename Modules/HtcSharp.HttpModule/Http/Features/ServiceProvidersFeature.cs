// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace HtcSharp.HttpModule.Http.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http\src\Features\ServiceProvidersFeature.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    public class ServiceProvidersFeature : IServiceProvidersFeature {
        public IServiceProvider RequestServices { get; set; }
    }
}