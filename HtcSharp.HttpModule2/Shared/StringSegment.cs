using System;
using System.Runtime.CompilerServices;

namespace HtcSharp.HttpModule2.Shared {
    public readonly struct StringSegment : IEquatable<StringSegment>, IEquatable<string> {

        public static readonly StringSegment Empty = string.Empty;

        public StringSegment(string buffer) {
            Buffer = buffer;
            Offset = 0;
            Length = buffer?.Length ?? 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringSegment(string buffer, int offset, int length) {
            if (buffer == null || (uint)offset > (uint)buffer.Length || (uint)length > (uint)(buffer.Length - offset)) {
                ThrowInvalidArguments(buffer, offset, length);
            }

            Buffer = buffer;
            Offset = offset;
            Length = length;
        }

        public string Buffer { get; }

        public int Offset { get; }

        public int Length { get; }

        public string Value {
            get {
                if (HasValue) {
                    return Buffer.Substring(Offset, Length);
                } else {
                    return null;
                }
            }
        }

        public bool HasValue {
            get { return Buffer != null; }
        }

        public char this[int index] {
            get {
                if ((uint)index >= (uint)Length) {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
                }

                return Buffer[Offset + index];
            }
        }

        public ReadOnlySpan<char> AsSpan() => Buffer.AsSpan(Offset, Length);

        public ReadOnlyMemory<char> AsMemory() => Buffer.AsMemory(Offset, Length);

        public static int Compare(StringSegment a, StringSegment b, StringComparison comparisonType) {
            var minLength = Math.Min(a.Length, b.Length);
            var diff = string.Compare(a.Buffer, a.Offset, b.Buffer, b.Offset, minLength, comparisonType);
            if (diff == 0) {
                diff = a.Length - b.Length;
            }

            return diff;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            return obj is StringSegment segment && Equals(segment);
        }

        public bool Equals(StringSegment other) => Equals(other, StringComparison.Ordinal);

        public bool Equals(StringSegment other, StringComparison comparisonType) {
            if (Length != other.Length) {
                return false;
            }

            return string.Compare(Buffer, Offset, other.Buffer, other.Offset, other.Length, comparisonType) == 0;
        }

        public static bool Equals(StringSegment a, StringSegment b, StringComparison comparisonType) {
            return a.Equals(b, comparisonType);
        }

        public bool Equals(string text) {
            return Equals(text, StringComparison.Ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(string text, StringComparison comparisonType) {
            if (text == null) {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            }

            var textLength = text.Length;
            if (!HasValue || Length != textLength) {
                return false;
            }

            return string.Compare(Buffer, Offset, text, 0, textLength, comparisonType) == 0;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() {
#if NETCOREAPP
            return string.GetHashCode(AsSpan());
#elif NETSTANDARD
            // This GetHashCode is expensive since it allocates on every call.
            // However this is required to ensure we retain any behavior (such as hash code randomization) that
            // string.GetHashCode has.
            return Value?.GetHashCode() ?? 0;
#else
#error Target frameworks need to be updated.
#endif

        }

        public static bool operator ==(StringSegment left, StringSegment right) => left.Equals(right);

        public static bool operator !=(StringSegment left, StringSegment right) => !left.Equals(right);

        public static implicit operator StringSegment(string value) => new StringSegment(value);

        public static implicit operator ReadOnlySpan<char>(StringSegment segment) => segment.AsSpan();

        public static implicit operator ReadOnlyMemory<char>(StringSegment segment) => segment.AsMemory();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool StartsWith(string text, StringComparison comparisonType) {
            if (text == null) {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            }

            var result = false;
            var textLength = text.Length;

            if (HasValue && Length >= textLength) {
                result = string.Compare(Buffer, Offset, text, 0, textLength, comparisonType) == 0;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EndsWith(string text, StringComparison comparisonType) {
            if (text == null) {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
            }

            var result = false;
            var textLength = text.Length;
            var comparisonLength = Offset + Length - textLength;

            if (HasValue && comparisonLength > 0) {
                result = string.Compare(Buffer, comparisonLength, text, 0, textLength, comparisonType) == 0;
            }

            return result;
        }

        public string Substring(int offset) => Substring(offset, Length - offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Substring(int offset, int length) {
            if (!HasValue || offset < 0 || length < 0 || (uint)(offset + length) > (uint)Length) {
                ThrowInvalidArguments(offset, length);
            }

            return Buffer.Substring(Offset + offset, length);
        }

        public StringSegment Subsegment(int offset) => Subsegment(offset, Length - offset);

        public StringSegment Subsegment(int offset, int length) {
            if (!HasValue || offset < 0 || length < 0 || (uint)(offset + length) > (uint)Length) {
                ThrowInvalidArguments(offset, length);
            }

            return new StringSegment(Buffer, Offset + offset, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(char c, int start, int count) {
            var offset = Offset + start;

            if (!HasValue || start < 0 || (uint)offset > (uint)Buffer.Length) {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
            }

            if (count < 0) {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count);
            }

            var index = Buffer.IndexOf(c, offset, count);
            if (index != -1) {
                index -= Offset;
            }

            return index;
        }

        public int IndexOf(char c, int start) => IndexOf(c, start, Length - start);

        public int IndexOf(char c) => IndexOf(c, 0, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOfAny(char[] anyOf, int startIndex, int count) {
            var index = -1;

            if (HasValue) {
                if (startIndex < 0 || Offset + startIndex > Buffer.Length) {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
                }

                if (count < 0 || Offset + startIndex + count > Buffer.Length) {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count);
                }

                index = Buffer.IndexOfAny(anyOf, Offset + startIndex, count);
                if (index != -1) {
                    index -= Offset;
                }
            }

            return index;
        }

        public int IndexOfAny(char[] anyOf, int startIndex) {
            return IndexOfAny(anyOf, startIndex, Length - startIndex);
        }

        public int IndexOfAny(char[] anyOf) {
            return IndexOfAny(anyOf, 0, Length);
        }

        public int LastIndexOf(char value) {
            var index = -1;

            if (HasValue) {
                index = Buffer.LastIndexOf(value, Offset + Length - 1, Length);
                if (index != -1) {
                    index -= Offset;
                }
            }

            return index;
        }

        public StringSegment Trim() => TrimStart().TrimEnd();

        public unsafe StringSegment TrimStart() {
            var trimmedStart = Offset;
            var length = Offset + Length;

            fixed (char* p = Buffer) {
                while (trimmedStart < length) {
                    var c = p[trimmedStart];

                    if (!char.IsWhiteSpace(c)) {
                        break;
                    }

                    trimmedStart++;
                }
            }

            return new StringSegment(Buffer, trimmedStart, length - trimmedStart);
        }

        public unsafe StringSegment TrimEnd() {
            var offset = Offset;
            var trimmedEnd = offset + Length - 1;

            fixed (char* p = Buffer) {
                while (trimmedEnd >= offset) {
                    var c = p[trimmedEnd];

                    if (!char.IsWhiteSpace(c)) {
                        break;
                    }

                    trimmedEnd--;
                }
            }

            return new StringSegment(Buffer, offset, trimmedEnd - offset + 1);
        }

        public StringTokenizer Split(char[] chars) {
            return new StringTokenizer(this, chars);
        }

        public static bool IsNullOrEmpty(StringSegment value) {
            var res = false;

            if (!value.HasValue || value.Length == 0) {
                res = true;
            }

            return res;
        }

        public override string ToString() {
            return Value ?? string.Empty;
        }

        private static void ThrowInvalidArguments(string buffer, int offset, int length) {
            throw GetInvalidArgumentsException();
            Exception GetInvalidArgumentsException() {
                if (buffer == null) {
                    return ThrowHelper.GetArgumentNullException(ExceptionArgument.buffer);
                }
                if (offset < 0) {
                    return ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.offset);
                }
                if (length < 0) {
                    return ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.length);
                }
                return ThrowHelper.GetArgumentException(ExceptionResource.Argument_InvalidOffsetLength);
            }
        }

        private void ThrowInvalidArguments(int offset, int length) {
            throw GetInvalidArgumentsException(HasValue);

            Exception GetInvalidArgumentsException(bool hasValue) {
                if (!hasValue) {
                    return ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.offset);
                }

                if (offset < 0) {
                    return ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.offset);
                }

                if (length < 0) {
                    return ThrowHelper.GetArgumentOutOfRangeException(ExceptionArgument.length);
                }

                return ThrowHelper.GetArgumentException(ExceptionResource.Argument_InvalidOffsetLengthStringSegment);
            }
        }
    }
}
