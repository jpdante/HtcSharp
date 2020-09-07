// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;

namespace HtcSharp.HttpModule.Http.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Features\src\IHttpRequestLifetimeFeature.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    public interface IHttpRequestLifetimeFeature {
        /// <summary>
        /// A <see cref="CancellationToken"/> that fires if the request is aborted and
        /// the application should cease processing. The token will not fire if the request
        /// completes successfully.
        /// </summary>
        CancellationToken RequestAborted { get; set; }

        /// <summary>
        /// Forcefully aborts the request if it has not already completed. This will result in
        /// RequestAborted being triggered.
        /// </summary>
        void Abort();
    }
}