// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Http.Abstractions.Routing {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Abstractions\src\Routing\IRouteValuesFeature.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    /// <summary>
    /// A feature interface for routing values. Use <see cref="HttpContext.Features"/>
    /// to access the values associated with the current request.
    /// </summary>
    public interface IRouteValuesFeature {
        /// <summary>
        /// Gets or sets the <see cref="RouteValueDictionary"/> associated with the currrent
        /// request.
        /// </summary>
        RouteValueDictionary RouteValues { get; set; }
    }
}