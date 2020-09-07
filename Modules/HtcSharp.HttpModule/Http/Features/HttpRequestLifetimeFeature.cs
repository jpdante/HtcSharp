// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;

namespace HtcSharp.HttpModule.Http.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http\src\Features\HttpRequestLifetimeFeature.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    public class HttpRequestLifetimeFeature : IHttpRequestLifetimeFeature {
        public CancellationToken RequestAborted { get; set; }

        public void Abort() {
        }
    }
}