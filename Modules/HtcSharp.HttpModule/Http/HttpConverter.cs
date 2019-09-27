using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace HtcSharp.HttpModule.Http {
    public class HttpConverter {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe HttpMethod GetHttpMethod(byte* data, int length, out int methodLength) {
            methodLength = 0;
            if (length > 8) return HttpMethod.CUSTOM;
            switch (*(ulong*)data) {
                case (ulong)HttpMethod.GET:
                    methodLength = 3;
                    return HttpMethod.GET;
                case (ulong)HttpMethod.POST:
                    methodLength = 4;
                    return HttpMethod.POST;
                case (ulong)HttpMethod.HEAD:
                    methodLength = 4;
                    return HttpMethod.HEAD;
                case (ulong)HttpMethod.PUT:
                    methodLength = 3;
                    return HttpMethod.PUT;
                case (ulong)HttpMethod.DELETE:
                    methodLength = 6;
                    return HttpMethod.DELETE;
                case (ulong)HttpMethod.TRACE:
                    methodLength = 5;
                    return HttpMethod.TRACE;
                case (ulong)HttpMethod.PATCH:
                    methodLength = 5;
                    return HttpMethod.PATCH;
                case (ulong)HttpMethod.CONNECT:
                    methodLength = 7;
                    return HttpMethod.CONNECT;
                case (ulong)HttpMethod.OPTIONS:
                    methodLength = 7;
                    return HttpMethod.OPTIONS;
                default:
                    return HttpMethod.CUSTOM;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe HttpVersion GetKnownVersion(byte* location, int length) {
            HttpVersion knownVersion;
            var version = *(ulong*)location;
            if (length < sizeof(ulong) + 1 || location[sizeof(ulong)] != (byte)'\r') {
                knownVersion = HttpVersion.UNKNOWN;
            } else switch (version) {
                case (ulong)HttpVersion.HTTP_11:
                    knownVersion = HttpVersion.HTTP_11;
                    break;
                case (ulong)HttpVersion.HTTP_2:
                    knownVersion = HttpVersion.HTTP_2;
                    break;
                case (ulong)HttpVersion.HTTP_10:
                    knownVersion = HttpVersion.HTTP_10;
                    break;
                default:
                    knownVersion = HttpVersion.UNKNOWN;
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