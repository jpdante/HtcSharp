using System;

namespace HtcSharp.HttpModule.Core.Http.Http {
    public interface IHttpRequestLineHandler {
        void OnStartLine(HttpMethod method, HttpVersion version, Span<byte> target, Span<byte> path, Span<byte> query, Span<byte> customMethod, bool pathEncoded);
    }
}
