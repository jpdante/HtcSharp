// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace HtcSharp.HttpModule.Connections.Abstractions.Exceptions {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Connections.Abstractions\src\Exceptions\ConnectionResetException.cs
    // Start-At-Remote-Line 8
    // SourceTools-End
    public class ConnectionResetException : IOException {
        public ConnectionResetException(string message) : base(message) {
        }

        public ConnectionResetException(string message, Exception inner) : base(message, inner) {
        }
    }
}