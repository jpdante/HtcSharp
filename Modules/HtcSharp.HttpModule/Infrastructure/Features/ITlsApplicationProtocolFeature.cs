using System;

namespace HtcSharp.HttpModule.Infrastructure.Features {
    public interface ITlsApplicationProtocolFeature {
        ReadOnlyMemory<byte> ApplicationProtocol { get; }
    }
}
