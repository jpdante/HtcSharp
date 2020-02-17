// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using TestLib.Features;
using TestLib.Http.Features;
using TestLib.Http.Features.Interfaces;
using TestLib.Http.Protocols.Http;
using TestLib.Infrastructure;
using TestLib.Net.Connections.Exceptions;

namespace TestLib.Http.Protocols.Http2 {
    internal partial class Http2Stream : IHttp2StreamIdFeature,
                                         IHttpMinRequestBodyDataRateFeature,
                                         IHttpResetFeature,
                                         IHttpResponseTrailersFeature {
        private IHeaderDictionary _userTrailers;

        IHeaderDictionary IHttpResponseTrailersFeature.Trailers {
            get {
                if (ResponseTrailers == null) {
                    ResponseTrailers = new HttpResponseTrailers();
                    if (HasResponseCompleted) {
                        ResponseTrailers.SetReadOnly();
                    }
                }
                return _userTrailers ?? ResponseTrailers;
            }
            set {
                _userTrailers = value;
            }
        }

        int IHttp2StreamIdFeature.StreamId => _context.StreamId;

        MinDataRate IHttpMinRequestBodyDataRateFeature.MinDataRate {
            get => throw new NotSupportedException(CoreStrings.Http2MinDataRateNotSupported);
            set {
                if (value != null) {
                    throw new NotSupportedException(CoreStrings.Http2MinDataRateNotSupported);
                }

                MinRequestBodyDataRate = value;
            }
        }

        void IHttpResetFeature.Reset(int errorCode) {
            var abortReason = new ConnectionAbortedException($@"The HTTP/2 stream was reset by the application with error code {(Http2ErrorCode)errorCode}.");
            ResetAndAbort(abortReason, (Http2ErrorCode)errorCode);
        }
    }
}