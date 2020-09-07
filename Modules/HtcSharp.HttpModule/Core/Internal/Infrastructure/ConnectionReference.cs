// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace HtcSharp.HttpModule.Core.Internal.Infrastructure {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Infrastructure\ConnectionReference.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    internal class ConnectionReference {
        private readonly WeakReference<KestrelConnection> _weakReference;

        public ConnectionReference(KestrelConnection connection) {
            _weakReference = new WeakReference<KestrelConnection>(connection);
            ConnectionId = connection.TransportConnection.ConnectionId;
        }

        public string ConnectionId { get; }

        public bool TryGetConnection(out KestrelConnection connection) {
            return _weakReference.TryGetTarget(out connection);
        }
    }
}