// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace HtcSharp.HttpModule.Transport.Sockets.Internal {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Transport.Sockets\src\Internal\ISocketsTrace.cs
    // Start-At-Remote-Line 8
    // SourceTools-End
    internal interface ISocketsTrace : ILogger {
        void ConnectionReadFin(string connectionId);

        void ConnectionWriteFin(string connectionId, string reason);

        void ConnectionError(string connectionId, Exception ex);

        void ConnectionReset(string connectionId);

        void ConnectionPause(string connectionId);

        void ConnectionResume(string connectionId);
    }
}