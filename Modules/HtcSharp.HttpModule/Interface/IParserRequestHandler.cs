using System;
using HtcSharp.HttpModule.Core.Http;

namespace HtcSharp.HttpModule.Interface {
    public interface IParserRequestHandler {
        void OnRequestStart(HttpMethod httpMethod, HttpVersion httpVersion, Span<byte> targetBuffer, Span<byte> pathBuffer, Span<byte> query, Span<byte> customMethod, bool pathEncoded);
        void OnRequestHeader(Span<byte> nameBuffer, Span<byte> valueBuffer);
        void OnRequestHeaderComplete();
    }
}
