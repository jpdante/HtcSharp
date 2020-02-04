using System;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http {
    public interface IHttpHeadersHandler {
        void OnHeader(Span<byte> name, Span<byte> value);
        void OnHeadersComplete();
    }
}