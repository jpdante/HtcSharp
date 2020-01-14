using System;
using System.Buffers;
using System.Reflection;
using System.Runtime.CompilerServices;
using HtcSharp.Core.Logging;
using HtcSharp.HttpModule.Helpers;
using HtcSharp.HttpModule.Interface;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

// ReSharper disable InconsistentNaming
namespace HtcSharp.HttpModule.Core.Http {
    public class HttpParser {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private const byte ByteCR = (byte)'\r';
        private const byte ByteLF = (byte)'\n';
        private const byte ByteColon = (byte)':';
        private const byte ByteSpace = (byte)' ';
        private const byte ByteTab = (byte)'\t';
        private const byte ByteQuestionMark = (byte)'?';
        private const byte BytePercentage = (byte)'%';
        private const int MinTlsRequestSize = 1;

        public unsafe bool ParseRequestLine(IParserRequestHandler handler, in ReadOnlySequence<byte> buffer, out SequencePosition consumed, out SequencePosition examined) {
            consumed = buffer.Start;
            examined = buffer.End;
            var span = buffer.First.Span;
            var lineIndex = span.IndexOf(ByteLF);
            if (lineIndex >= 0) {
                consumed = buffer.GetPosition(lineIndex + 1, consumed);
                span = span.Slice(0, lineIndex + 1);
            } else if (buffer.IsSingleSegment) {
                Logger.Debug("IsSingleSegment");
                return false;
            } else if (TryGetNewLine(buffer, out var found)) {
                span = buffer.Slice(consumed, found).ToSpan();
                consumed = found;
            } else {
                Logger.Debug("lineIndex <! 0");
                return false;
            }
            fixed (byte* data = span) {
                ParseRequestLine(handler, data, span.Length);
            }
            examined = consumed;
            return true;
        }

        private unsafe void ParseRequestLine(IParserRequestHandler handler, byte* data, int length) {
            var method = HttpConverter.GetHttpMethod(data, length, out var pathStartOffset);
            Span<byte> customMethod = default;
            if (method == HttpMethod.Custom) {
                customMethod = GetUnknownMethod(data, length, out pathStartOffset);
            }
            var offset = pathStartOffset + 1;
            if (offset >= length) {
                RejectRequestLine(data, length);
            }
            var ch = data[offset];
            if (ch == ByteSpace || ch == ByteQuestionMark || ch == BytePercentage) {
                RejectRequestLine(data, length);
            }
            var pathEncoded = false;
            var pathStart = offset;
            offset++;
            for (; offset < length; offset++) {
                ch = data[offset];
                if (ch == ByteSpace || ch == ByteQuestionMark) {
                    break;
                }
                if (ch == BytePercentage) {
                    pathEncoded = true;
                }
            }
            var pathBuffer = new Span<byte>(data + pathStart, offset - pathStart);
            var queryStart = offset;
            if (ch == ByteQuestionMark) {
                for (; offset < length; offset++) {
                    ch = data[offset];
                    if (ch == ByteSpace) {
                        break;
                    }
                }
            }
            if (offset == length) {
                RejectRequestLine(data, length);
            }
            var targetBuffer = new Span<byte>(data + pathStart, offset - pathStart);
            var query = new Span<byte>(data + queryStart, offset - queryStart);
            offset++;
            var httpVersion = HttpConverter.GetKnownVersion(data + offset, length - offset);
            if (httpVersion == HttpVersion.Unknown) {
                if (data[offset] == ByteCR || data[length - 2] != ByteCR) {
                    RejectRequestLine(data, length);
                } else {
                    RejectUnknownVersion(data + offset, length - offset - 2);
                }
            }
            if (data[offset + 8 + 1] != ByteLF) {
                RejectRequestLine(data, length);
            }
            handler.OnRequestStart(method, httpVersion, targetBuffer, pathBuffer, query, customMethod, pathEncoded);
        }

        public unsafe bool ParseHeaders(IParserRequestHandler handler, ref SequenceReader<byte> reader) {
            while (!reader.End) {
                var span = reader.UnreadSpan;
                while (span.Length > 0) {
                    var ch1 = (byte)0;
                    var ch2 = (byte)0;
                    var readAhead = 0;
                    if (span.Length >= 2) {
                        ch1 = span[0];
                        ch2 = span[1];
                    } else if (reader.TryRead(out ch1)) {
                        readAhead = (reader.TryRead(out ch2)) ? 2 : 1;
                    }
                    if (ch1 == ByteCR) {
                        if (ch2 == ByteLF) {
                            if (readAhead == 0) {
                                reader.Advance(2);
                            }
                            handler.OnRequestHeaderComplete();
                            return true;
                        } else if (readAhead == 1) {
                            reader.Rewind(1);
                            return false;
                        }
                        //Debug.Assert(readAhead == 0 || readAhead == 2);
                        //BadHttpRequestException.Throw(RequestRejectionReason.InvalidRequestHeadersNoCRLF);
                        throw new Exception("Invalid request headers no CRLF");
                    }
                    var length = 0;
                    if (readAhead == 0) {
                        length = span.IndexOf(ByteLF) + 1;
                        if (length > 0) {
                            fixed (byte* pHeader = span) {
                                TakeSingleHeader(pHeader, length, handler);
                            }
                            reader.Advance(length);
                            span = span.Slice(length);
                        }
                    }
                    if (length <= 0) {
                        if (readAhead > 0) {
                            reader.Rewind(readAhead);
                        }
                        length = ParseMultiSpanHeader(handler, ref reader);
                        if (length < 0) {
                            return false;
                        }
                        reader.Advance(length);
                        span = default;
                    }
                }
            }

            return false;
        }

        private unsafe int ParseMultiSpanHeader(IParserRequestHandler handler, ref SequenceReader<byte> reader) {
            var buffer = reader.Sequence;
            var currentSlice = buffer.Slice(reader.Position, reader.Remaining);
            var lineEndPosition = currentSlice.PositionOf(ByteLF);
            if (lineEndPosition == null) return -1;
            var lineEnd = lineEndPosition.Value;
            lineEnd = buffer.GetPosition(1, lineEnd);
            var headerSpan = buffer.Slice(reader.Position, lineEnd).ToSpan();
            var length = headerSpan.Length;
            fixed (byte* pHeader = headerSpan) {
                TakeSingleHeader(pHeader, length, handler);
            }
            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void TakeSingleHeader(byte* headerLine, int length, IParserRequestHandler handler) {
            var valueEnd = length - 3;
            var nameEnd = FindEndOfName(headerLine, length);
            if (nameEnd <= 0 || headerLine[valueEnd + 2] != ByteLF || headerLine[valueEnd + 1] != ByteCR) {
                RejectRequestHeader(headerLine, length);
            }
            var valueStart = nameEnd + 1;
            for (; valueStart < valueEnd; valueStart++) {
                var ch = headerLine[valueStart];
                if (ch != ByteTab && ch != ByteSpace && ch != ByteCR) {
                    break;
                } else if (ch == ByteCR) {
                    RejectRequestHeader(headerLine, length);
                }
            }
            var valueBuffer = new Span<byte>(headerLine + valueStart, valueEnd - valueStart + 1);
            if (valueBuffer.Contains(ByteCR)) {
                RejectRequestHeader(headerLine, length);
            }
            var lengthChanged = false;
            for (; valueEnd >= valueStart; valueEnd--) {
                var ch = headerLine[valueEnd];
                if (ch != ByteTab && ch != ByteSpace) {
                    break;
                }
                lengthChanged = true;
            }
            if (lengthChanged) {
                valueBuffer = new Span<byte>(headerLine + valueStart, valueEnd - valueStart + 1);
            }
            var nameBuffer = new Span<byte>(headerLine, nameEnd);
            handler.OnRequestHeader(nameBuffer, valueBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe int FindEndOfName(byte* headerLine, int length) {
            var index = 0;
            var sawWhitespace = false;
            for (; index < length; index++) {
                var ch = headerLine[index];
                if (ch == ByteColon) {
                    break;
                }
                if (ch == ByteTab || ch == ByteSpace || ch == ByteCR) {
                    sawWhitespace = true;
                }
            }
            if (index == length || sawWhitespace) index = -1;
            return index;
        }

        private static unsafe bool IsTlsHandshake(byte* data, int length) {
            const byte SslRecordTypeHandshake = (byte)0x16;
            return (length >= MinTlsRequestSize && data[0] == SslRecordTypeHandshake);
        }

        private unsafe void RejectRequestLine(byte* requestLine, int length) {
            if (IsTlsHandshake(requestLine, length)) throw new Exception("Request using https instead of http.");
            throw new Exception("Unknown request error.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool TryGetNewLine(in ReadOnlySequence<byte> buffer, out SequencePosition found) {
            var byteLfPosition = buffer.PositionOf(ByteLF);
            if (byteLfPosition != null) {
                found = buffer.GetPosition(1, byteLfPosition.Value);
                return true;
            }

            found = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private unsafe Span<byte> GetUnknownMethod(byte* data, int length, out int methodLength) {
            var invalidIndex = HttpCharacters.IndexOfInvalidTokenChar(data, length);
            if (invalidIndex <= 0 || data[invalidIndex] != ByteSpace) {
                RejectRequestLine(data, length);
            }
            methodLength = invalidIndex;
            return new Span<byte>(data, methodLength);
        }

        private unsafe void RejectUnknownVersion(byte* version, int length) => throw new Exception("Unknown http request version.");
        private unsafe void RejectRequestHeader(byte* headerLine, int length) => throw new Exception("Invalid http request header.");
    }
}
