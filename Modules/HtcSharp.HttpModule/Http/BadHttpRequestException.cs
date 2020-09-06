// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Runtime.CompilerServices;
using HtcSharp.HttpModule.Core;
using HtcSharp.HttpModule.Core.Internal.Infrastructure;
using Microsoft.Extensions.Primitives;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Protocols.Http;

namespace HtcSharp.HttpModule.Http {
    public sealed class BadHttpRequestException : IOException {
        private BadHttpRequestException(string message, int statusCode, RequestRejectionReason reason)
            : this(message, statusCode, reason, null) {
        }

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
        internal static void Throw(RequestRejectionReason reason, HttpMethod method)
            => throw GetException(reason, method.ToString().ToUpperInvariant());

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static BadHttpRequestException GetException(RequestRejectionReason reason) {
            BadHttpRequestException ex;
            switch (reason) {
                case RequestRejectionReason.InvalidRequestHeadersNoCRLF:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_InvalidRequestHeadersNoCRLF, StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidRequestLine:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_InvalidRequestLine, StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.MalformedRequestInvalidHeaders:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_MalformedRequestInvalidHeaders, StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.MultipleContentLengths:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_MultipleContentLengths, StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.UnexpectedEndOfRequestContent:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_UnexpectedEndOfRequestContent, StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.BadChunkSuffix:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_BadChunkSuffix, StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.BadChunkSizeData:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_BadChunkSizeData, StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.ChunkedRequestIncomplete:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_ChunkedRequestIncomplete, StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidCharactersInHeaderName:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_InvalidCharactersInHeaderName, StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.RequestLineTooLong:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_RequestLineTooLong, StatusCodes.Status414UriTooLong, reason);
                    break;
                case RequestRejectionReason.HeadersExceedMaxTotalSize:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_HeadersExceedMaxTotalSize, StatusCodes.Status431RequestHeaderFieldsTooLarge, reason);
                    break;
                case RequestRejectionReason.TooManyHeaders:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_TooManyHeaders, StatusCodes.Status431RequestHeaderFieldsTooLarge, reason);
                    break;
                case RequestRejectionReason.RequestBodyTooLarge:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_RequestBodyTooLarge, StatusCodes.Status413PayloadTooLarge, reason);
                    break;
                case RequestRejectionReason.RequestHeadersTimeout:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_RequestHeadersTimeout, StatusCodes.Status408RequestTimeout, reason);
                    break;
                case RequestRejectionReason.RequestBodyTimeout:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_RequestBodyTimeout, StatusCodes.Status408RequestTimeout, reason);
                    break;
                case RequestRejectionReason.OptionsMethodRequired:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_MethodNotAllowed, StatusCodes.Status405MethodNotAllowed, reason, HttpMethod.Options);
                    break;
                case RequestRejectionReason.ConnectMethodRequired:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_MethodNotAllowed, StatusCodes.Status405MethodNotAllowed, reason, HttpMethod.Connect);
                    break;
                case RequestRejectionReason.MissingHostHeader:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_MissingHostHeader, StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.MultipleHostHeaders:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_MultipleHostHeaders, StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidHostHeader:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_InvalidHostHeader, StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.UpgradeRequestCannotHavePayload:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest_UpgradeRequestCannotHavePayload, StatusCodes.Status400BadRequest, reason);
                    break;
                default:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest, StatusCodes.Status400BadRequest, reason);
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
                case RequestRejectionReason.InvalidRequestLine:
                    ex = new BadHttpRequestException($@"Invalid request line: '{detail}'", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidRequestTarget:
                    ex = new BadHttpRequestException($@"Invalid request target: '{detail}'", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidRequestHeader:
                    ex = new BadHttpRequestException($@"Invalid request header: '{detail}'", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidContentLength:
                    ex = new BadHttpRequestException($@"Invalid content length: {detail}", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.UnrecognizedHTTPVersion:
                    ex = new BadHttpRequestException($@"Unrecognized HTTP version: '{detail}'", StatusCodes.Status505HttpVersionNotsupported, reason);
                    break;
                case RequestRejectionReason.FinalTransferCodingNotChunked:
                    ex = new BadHttpRequestException($@"The message body length cannot be determined because the final transfer coding was set to '{detail}' instead of 'chunked'.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.LengthRequired:
                    ex = new BadHttpRequestException($@"{detail} request contains no Content-Length or Transfer-Encoding header.", StatusCodes.Status411LengthRequired, reason);
                    break;
                case RequestRejectionReason.LengthRequiredHttp10:
                    ex = new BadHttpRequestException($@"{detail} request contains no Content-Length header.", StatusCodes.Status400BadRequest, reason);
                    break;
                case RequestRejectionReason.InvalidHostHeader:
                    ex = new BadHttpRequestException($@"Invalid Host header: '{detail}'", StatusCodes.Status400BadRequest, reason);
                    break;
                default:
                    ex = new BadHttpRequestException(CoreStrings.BadRequest, StatusCodes.Status400BadRequest, reason);
                    break;
            }

            return ex;
        }
    }
}