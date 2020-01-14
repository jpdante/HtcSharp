using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace HtcSharp.HttpModule.Core.Http {
    public class HttpConverter {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe HttpMethod GetHttpMethod(byte* data, int length, out int methodLength) {
            methodLength = 0;
            if (length > 8) return HttpMethod.Custom;
            switch (*(ulong*)data) {
                case (ulong)HttpMethod.Get:
                    methodLength = 3;
                    return HttpMethod.Get;
                case (ulong)HttpMethod.Post:
                    methodLength = 4;
                    return HttpMethod.Post;
                case (ulong)HttpMethod.Head:
                    methodLength = 4;
                    return HttpMethod.Head;
                case (ulong)HttpMethod.Put:
                    methodLength = 3;
                    return HttpMethod.Put;
                case (ulong)HttpMethod.Delete:
                    methodLength = 6;
                    return HttpMethod.Delete;
                case (ulong)HttpMethod.Trace:
                    methodLength = 5;
                    return HttpMethod.Trace;
                case (ulong)HttpMethod.Patch:
                    methodLength = 5;
                    return HttpMethod.Patch;
                case (ulong)HttpMethod.Connect:
                    methodLength = 7;
                    return HttpMethod.Connect;
                case (ulong)HttpMethod.Options:
                    methodLength = 7;
                    return HttpMethod.Options;
                default:
                    return HttpMethod.Custom;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe HttpVersion GetKnownVersion(byte* location, int length) {
            HttpVersion knownVersion;
            var version = *(ulong*)location;
            if (length < sizeof(ulong) + 1 || location[sizeof(ulong)] != (byte)'\r') {
                knownVersion = HttpVersion.Unknown;
            } else switch (version) {
                case (ulong)HttpVersion.Http11:
                    knownVersion = HttpVersion.Http11;
                    break;
                case (ulong)HttpVersion.Http2:
                    knownVersion = HttpVersion.Http2;
                    break;
                case (ulong)HttpVersion.Http10:
                    knownVersion = HttpVersion.Http10;
                    break;
                default:
                    knownVersion = HttpVersion.Unknown;
                    break;
            }
            return knownVersion;
        }

        public static unsafe ulong GetAsciiStringAsLong(string str) {
            var bytes = Encoding.ASCII.GetBytes(str);
            fixed (byte* ptr = &bytes[0]) {
                return *(ulong*)ptr;
            }
        }

        public static unsafe string GetLongAsAsciiString(ulong str) {
            return Encoding.ASCII.GetString(BitConverter.GetBytes(str));
        }

        public static unsafe uint GetAsciiStringAsInt(string str) {
            var bytes = Encoding.ASCII.GetBytes(str);
            fixed (byte* ptr = &bytes[0]) {
                return *(uint*)ptr;
            }
        }
    }
}