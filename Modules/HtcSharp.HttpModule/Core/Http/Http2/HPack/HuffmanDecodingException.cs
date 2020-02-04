using System;

namespace HtcSharp.HttpModule.Core.Http.Http2.HPack {
    internal sealed class HuffmanDecodingException : Exception {
        public HuffmanDecodingException(string message)
            : base(message) {
        }
    }
}
