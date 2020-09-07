// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Buffers;

namespace System.Buffers {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Shared\Buffers.MemoryPool\MemoryPoolFactory.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    internal static class SlabMemoryPoolFactory {
        public static MemoryPool<byte> Create() {
#if DEBUG
            return new DiagnosticMemoryPool(CreateSlabMemoryPool());
#else
            return CreateSlabMemoryPool();
#endif
        }

        public static MemoryPool<byte> CreateSlabMemoryPool() {
            return new SlabMemoryPool();
        }
    }
}