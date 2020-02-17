// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace HtcSharp.HttpModule2.Net.Connections.Exceptions {
    public class AddressInUseException : InvalidOperationException {
        public AddressInUseException(string message) : base(message) {
        }

        public AddressInUseException(string message, Exception inner) : base(message, inner) {
        }
    }
}
