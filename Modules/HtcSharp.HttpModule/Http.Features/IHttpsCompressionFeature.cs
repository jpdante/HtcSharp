// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Http.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Features\src\IHttpsCompressionFeature.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    /// <summary>
    /// Configures response compression behavior for HTTPS on a per-request basis.
    /// </summary>
    public interface IHttpsCompressionFeature {
        /// <summary>
        /// The <see cref="HttpsCompressionMode"/> to use.
        /// </summary>
        HttpsCompressionMode Mode { get; set; }
    }
}