// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;

namespace HtcSharp.HttpModule.Transport.Sockets.Internal {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Transport.Sockets\src\Internal\BufferExtensions.cs
    // Start-At-Remote-Line 8
    // SourceTools-End
    internal static class BufferExtensions {
        public static ArraySegment<byte> GetArray(this Memory<byte> memory) {
            return ((ReadOnlyMemory<byte>) memory).GetArray();
        }

        public static ArraySegment<byte> GetArray(this ReadOnlyMemory<byte> memory) {
            if (!MemoryMarshal.TryGetArray(memory, out var result)) {
                throw new InvalidOperationException("Buffer backed by array was expected");
            }

            return result;
        }
    }
}