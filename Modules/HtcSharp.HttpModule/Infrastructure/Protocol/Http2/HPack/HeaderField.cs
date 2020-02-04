using System;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http2.HPack {
    internal readonly struct HeaderField {
        // http://httpwg.org/specs/rfc7541.html#rfc.section.4.1
        public const int RfcOverhead = 32;

        public HeaderField(Span<byte> name, Span<byte> value) {
            Name = new byte[name.Length];
            name.CopyTo(Name);

            Value = new byte[value.Length];
            value.CopyTo(Value);
        }

        public byte[] Name { get; }

        public byte[] Value { get; }

        public int Length => GetLength(Name.Length, Value.Length);

        public static int GetLength(int nameLength, int valueLength) => nameLength + valueLength + 32;
    }
}
