using System.Buffers;

namespace HtcSharp.HttpModule.Infrastructure.Features {
    public interface IMemoryPoolFeature {
        MemoryPool<byte> MemoryPool { get; }
    }
}
