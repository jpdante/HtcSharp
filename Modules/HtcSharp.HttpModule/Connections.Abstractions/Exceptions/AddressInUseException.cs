// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace HtcSharp.HttpModule.Connections.Abstractions.Exceptions {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Connections.Abstractions\src\Exceptions\AddressInUseException.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    public class AddressInUseException : InvalidOperationException {
        public AddressInUseException(string message) : base(message) {
        }

        public AddressInUseException(string message, Exception inner) : base(message, inner) {
        }
    }
}