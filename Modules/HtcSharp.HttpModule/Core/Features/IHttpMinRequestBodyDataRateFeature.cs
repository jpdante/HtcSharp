// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Core.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Features\IHttpMinRequestBodyDataRateFeature.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    /// <summary>
    /// Feature to set the minimum data rate at which the the request body must be sent by the client.
    /// This feature is not supported for HTTP/2 requests except to disable it entirely by setting <see cref="MinDataRate"/> to <see langword="null"/> 
    /// Instead, use <see cref="KestrelServerLimits.MinRequestBodyDataRate"/> for server-wide configuration which applies to both HTTP/2 and HTTP/1.x.
    /// </summary>
    public interface IHttpMinRequestBodyDataRateFeature {
        /// <summary>
        /// The minimum data rate in bytes/second at which the request body must be sent by the client.
        /// Setting this property to null indicates no minimum data rate should be enforced.
        /// This limit has no effect on upgraded connections which are always unlimited.
        /// This feature is not supported for HTTP/2 requests except to disable it entirely by setting <see cref="MinDataRate"/> to <see langword="null"/> 
        /// Instead, use <see cref="KestrelServerLimits.MinRequestBodyDataRate"/> for server-wide configuration which applies to both HTTP/2 and HTTP/1.x.
        /// </summary>
        MinDataRate MinDataRate { get; set; }
    }
}