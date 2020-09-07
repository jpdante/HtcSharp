using System;
using System.Buffers;

namespace HtcSharp.HttpModule.Core.Internal {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\MemoryPoolExtensions.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
    internal static class MemoryPoolExtensions {
        /// <summary>
        /// Computes a minimum segment size
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        public static int GetMinimumSegmentSize(this MemoryPool<byte> pool) {
            if (pool == null) {
                return 4096;
            }

            return Math.Min(4096, pool.MaxBufferSize);
        }

        public static int GetMinimumAllocSize(this MemoryPool<byte> pool) {
            // 1/2 of a segment
            return pool.GetMinimumSegmentSize() / 2;
        }
    }
}