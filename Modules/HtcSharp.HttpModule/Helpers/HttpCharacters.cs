using System;
using System.Runtime.CompilerServices;

namespace HtcSharp.HttpModule.Helpers {
    internal static class HttpCharacters {
        private static readonly int _tableSize = 128;
        private static readonly bool[] AlphaNumeric = InitializeAlphaNumeric();
        private static readonly bool[] Authority = InitializeAuthority();
        private static readonly bool[] Token = InitializeToken();
        private static readonly bool[] Host = InitializeHost();
        private static readonly bool[] FieldValue = InitializeFieldValue();

        internal static void Initialize() {
            var initialize = AlphaNumeric;
        }

        private static bool[] InitializeAlphaNumeric() {
            var alphaNumeric = new bool[_tableSize];
            for (var c = '0'; c <= '9'; c++) {
                alphaNumeric[c] = true;
            }
            for (var c = 'A'; c <= 'Z'; c++) {
                alphaNumeric[c] = true;
            }
            for (var c = 'a'; c <= 'z'; c++) {
                alphaNumeric[c] = true;
            }
            return alphaNumeric;
        }

        private static bool[] InitializeAuthority() {
            var authority = new bool[_tableSize];
            Array.Copy(AlphaNumeric, authority, _tableSize);
            authority[':'] = true;
            authority['.'] = true;
            authority['['] = true;
            authority[']'] = true;
            authority['@'] = true;
            return authority;
        }

        private static bool[] InitializeToken() {
            var token = new bool[_tableSize];
            Array.Copy(AlphaNumeric, token, _tableSize);
            token['!'] = true;
            token['#'] = true;
            token['$'] = true;
            token['%'] = true;
            token['&'] = true;
            token['\''] = true;
            token['*'] = true;
            token['+'] = true;
            token['-'] = true;
            token['.'] = true;
            token['^'] = true;
            token['_'] = true;
            token['`'] = true;
            token['|'] = true;
            token['~'] = true;
            return token;
        }

        private static bool[] InitializeHost() {
            var host = new bool[_tableSize];
            Array.Copy(AlphaNumeric, host, _tableSize);
            host['!'] = true;
            host['$'] = true;
            host['&'] = true;
            host['\''] = true;
            host['('] = true;
            host[')'] = true;
            host['-'] = true;
            host['.'] = true;
            host['_'] = true;
            host['~'] = true;
            return host;
        }

        private static bool[] InitializeFieldValue() {
            var fieldValue = new bool[_tableSize];
            for (var c = 0x20; c <= 0x7e; c++) {
                fieldValue[c] = true;
            }
            return fieldValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsInvalidAuthorityChar(Span<byte> s) {
            var authority = Authority;
            foreach (var c in s) {
                if (c >= (uint)authority.Length || !authority[c]) {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfInvalidHostChar(string s) {
            var host = Host;
            for (var i = 0; i < s.Length; i++) {
                var c = s[i];
                if (c >= (uint)host.Length || !host[c]) {
                    return i;
                }
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfInvalidTokenChar(string s) {
            var token = Token;
            for (var i = 0; i < s.Length; i++) {
                var c = s[i];
                if (c >= (uint)token.Length || !token[c]) {
                    return i;
                }
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once ArrangeModifiersOrder
        public unsafe static int IndexOfInvalidTokenChar(byte* s, int length) {
            var token = Token;
            for (var i = 0; i < length; i++) {
                var c = s[i];
                if (c >= (uint)token.Length || !token[c]) {
                    return i;
                }
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfInvalidFieldValueChar(string s) {
            var fieldValue = FieldValue;

            for (var i = 0; i < s.Length; i++) {
                var c = s[i];
                if (c >= (uint)fieldValue.Length || !fieldValue[c]) {
                    return i;
                }
            }

            return -1;
        }
    }
}