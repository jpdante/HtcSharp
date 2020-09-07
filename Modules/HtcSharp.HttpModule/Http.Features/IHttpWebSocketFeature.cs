// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.WebSockets;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Http.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Features\src\IHttpWebSocketFeature.cs
    // Start-At-Remote-Line 8
    // SourceTools-End
    public interface IHttpWebSocketFeature {
        /// <summary>
        /// Indicates if this is a WebSocket upgrade request.
        /// </summary>
        bool IsWebSocketRequest { get; }

        /// <summary>
        /// Attempts to upgrade the request to a <see cref="WebSocket"/>. Check <see cref="IsWebSocketRequest"/>
        /// before invoking this.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<WebSocket> AcceptAsync(WebSocketAcceptContext context);
    }
}