// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace HtcSharp.HttpModule.Core.Internal.Http2.HPack {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http2\HPack\HuffmanDecodingException.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    internal sealed class HuffmanDecodingException : Exception {
        public HuffmanDecodingException(string message)
            : base(message) {
        }
    }
}