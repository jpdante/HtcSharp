// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Http.Abstractions.Routing {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Abstractions\src\Routing\IEndpointFeature.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    /// <summary>
    /// A feature interface for endpoint routing. Use <see cref="HttpContext.Features"/>
    /// to access an instance associated with the current request.
    /// </summary>
    public interface IEndpointFeature {
        /// <summary>
        /// Gets or sets the selected <see cref="Http.Endpoint"/> for the current
        /// request.
        /// </summary>
        Endpoint Endpoint { get; set; }
    }
}