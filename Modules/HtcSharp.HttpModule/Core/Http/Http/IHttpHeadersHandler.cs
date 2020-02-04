using System;

namespace HtcSharp.HttpModule.Core.Http.Http {
    public interface IHttpHeadersHandler {
        void OnHeader(Span<byte> name, Span<byte> value);
        void OnHeadersComplete();
    }
}