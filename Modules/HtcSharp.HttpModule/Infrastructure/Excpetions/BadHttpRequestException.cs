using System.IO;
using System.Runtime.CompilerServices;
using HtcSharp.HttpModule.Core.Http.Http;
using HtcSharp.HttpModule.Core.Infrastructure;
using HtcSharp.HttpModule.Http.Http.Abstractions;
using HtcSharp.HttpModule.Infrastructure.Attibutes;
using Microsoft.Extensions.Primitives;

namespace HtcSharp.HttpModule.Infrastructure.Excpetions {
    public sealed class BadHttpRequestException : IOException {
        private BadHttpRequestException(string message, int statusCode, RequestRejectionReason reason)
            : this(message, statusCode, reason, null) { }

        private BadHttpRequestException(string message, int statusCode, RequestRejectionReason reason, HttpMethod? requiredMethod)
            : base(message) {
            StatusCode = statusCode;
            Reason = reason;

            if (requiredMethod.HasValue) {
                AllowedHeader = HttpUtilities.MethodToString(requiredMethod.Value);
            }
        }

        public int StatusCode { get; }

        internal StringValues AllowedHeader { get; }

        internal RequestRejectionReason Reason { get; }

        [StackTraceHidden]
        internal static void Throw(RequestRejectionReason reason) {
            throw GetException(reason);
        }

        [StackTraceHidden]
        internal static void Throw(RequestRejectionReason reason, HttpMethod method) => throw GetException(reason, method.ToString().ToUpperInvariant());

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static BadHttpRequestException GetException(RequestRejectionReason reason) {
            BadHttpRequestException ex;
            switch (reason) {
                case RequestRejectionReason.InvalidRequestHeadersNoCRLF:
                    ex = new BadHttpRequestException("Invalid request headers: missing final CRLF in header fields.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidRequestLine:
                    ex = new BadHttpRequestException("Invalid request line.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.MalformedRequestInvalidHeaders:
                    ex = new BadHttpRequestException("Malformed request: invalid headers.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.MultipleContentLengths:
                    ex = new BadHttpRequestException("Multiple Content-Length headers.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.UnexpectedEndOfRequestContent:
                    ex = new BadHttpRequestException("Unexpected end of request content.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.BadChunkSuffix:
                    ex = new BadHttpRequestException("Bad chunk suffix.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.BadChunkSizeData:
                    ex = new BadHttpRequestException("Bad chunk size data.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.ChunkedRequestIncomplete:
                    ex = new BadHttpRequestException("Chunked request incomplete.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidCharactersInHeaderName:
                    ex = new BadHttpRequestException("Invalid characters in header name.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.RequestLineTooLong:
                    ex = new BadHttpRequestException("Request line too long.", StatusCodes.Status414UriTooLong, reason);
                    break;
                case RequestRejectionReason.HeadersExceedMaxTotalSize:
                    ex = new BadHttpRequestException("Request headers too long.", StatusCodes.Status431RequestHeaderFieldsTooLarge, reason);
                    break;
                case RequestRejectionReason.TooManyHeaders:
                    ex = new BadHttpRequestException("Request contains too many headers.", StatusCodes.Status431RequestHeaderFieldsTooLarge, reason);
                    break;
                case RequestRejectionReason.RequestBodyTooLarge:
                    ex = new BadHttpRequestException("Request body too large.", StatusCodes.Status413PayloadTooLarge, reason);
                    break;
                case RequestRejectionReason.RequestHeadersTimeout:
                    ex = new BadHttpRequestException("Reading the request headers timed out.", StatusCodes.Status408RequestTimeout, reason);
                    break;
                case RequestRejectionReason.RequestBodyTimeout:
                    ex = new BadHttpRequestException("Reading the request body timed out due to data arriving too slowly. See MinRequestBodyDataRate.", StatusCodes.Status408RequestTimeout, reason);
                    break;
                case RequestRejectionReason.OptionsMethodRequired:
                    ex = new BadHttpRequestException("Method not allowed.", StatusCodes.Status405MethodNotAllowed, reason, HttpMethod.Options);
                    break;
                case RequestRejectionReason.ConnectMethodRequired:
                    ex = new BadHttpRequestException("Method not allowed.", StatusCodes.Status405MethodNotAllowed, reason, HttpMethod.Connect);
                    break;
                case RequestRejectionReason.MissingHostHeader:
                    ex = new BadHttpRequestException("Request is missing Host header.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.MultipleHostHeaders:
                    ex = new BadHttpRequestException("Multiple Host headers.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidHostHeader:
                    ex = new BadHttpRequestException("Invalid Host header.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.UpgradeRequestCannotHavePayload:
                    ex = new BadHttpRequestException("Requests with 'Connection: Upgrade' cannot have content in the request body.", StatusCodes.Status400BadRequest, reason);
                    break;
                default:
                    ex = new BadHttpRequestException("Bad request.", StatusCodes.Status400BadRequest, reason);
                    break;
            }
            return ex;
        }

        [StackTraceHidden]
        internal static void Throw(RequestRejectionReason reason, string detail) {
            throw GetException(reason, detail);
        }

        [StackTraceHidden]
        internal static void Throw(RequestRejectionReason reason, StringValues detail) {
            throw GetException(reason, detail.ToString());
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static BadHttpRequestException GetException(RequestRejectionReason reason, string detail) {
            BadHttpRequestException ex;
            switch (reason) {
                // TODO: Fix functions
                /*case RequestRejectionReason.InvalidRequestLine:
                    ex = new BadHttpRequestException(CoreStrings.FormatBadRequest_InvalidRequestLine_Detail(detail), StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidRequestTarget:
                    ex = new BadHttpRequestException(CoreStrings.FormatBadRequest_InvalidRequestTarget_Detail(detail), StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidRequestHeader:
                    ex = new BadHttpRequestException(CoreStrings.FormatBadRequest_InvalidRequestHeader_Detail(detail), StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidContentLength:
                    ex = new BadHttpRequestException(CoreStrings.FormatBadRequest_InvalidContentLength_Detail(detail), StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.UnrecognizedHTTPVersion:
                    ex = new BadHttpRequestException(CoreStrings.FormatBadRequest_UnrecognizedHTTPVersion(detail), StatusCodes.Status505HttpVersionNotsupported, reason);
                    break;
                case RequestRejectionReason.FinalTransferCodingNotChunked:
                    ex = new BadHttpRequestException(CoreStrings.FormatBadRequest_FinalTransferCodingNotChunked(detail), StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.LengthRequired:
                    ex = new BadHttpRequestException(CoreStrings.FormatBadRequest_LengthRequired(detail), StatusCodes.Status411LengthRequired, reason);
                    break;
                case RequestRejectionReason.LengthRequiredHttp10:
                    ex = new BadHttpRequestException(CoreStrings.FormatBadRequest_LengthRequiredHttp10(detail), StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidHostHeader:
                    ex = new BadHttpRequestException(CoreStrings.FormatBadRequest_InvalidHostHeader_Detail(detail), StatusCodes.Status400BadRequest, reason);
                    break;*/
                default:
                    ex = new BadHttpRequestException("Bad request.", StatusCodes.Status400BadRequest, reason);
                    break;
            }
            return ex;
        }
    }
}
