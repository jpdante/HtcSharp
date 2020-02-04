using System;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http2.HPack {
    internal sealed class HuffmanDecodingException : Exception {
        public HuffmanDecodingException(string message)
            : base(message) {
        }
    }
}
