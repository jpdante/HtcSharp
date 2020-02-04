using System;
using System.Collections.Generic;
using System.Text;
using HtcSharp.HttpModule.Core.Http.Http;
using HtcSharp.HttpModule.Infrastructure.Features;
using HtcSharp.HttpModule.Infrastructure.Heart;

namespace HtcSharp.HttpModule.Core.Http.Http2 {
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
            get => throw new NotSupportedException("This feature is not supported for HTTP/2 requests except to disable it entirely by setting the rate to null.");
            set {
                if (value != null) {
                    throw new NotSupportedException("This feature is not supported for HTTP/2 requests except to disable it entirely by setting the rate to null.");
                }

                MinRequestBodyDataRate = value;
            }
        }

        void IHttpResetFeature.Reset(int errorCode) {
            var abortReason = new ConnectionAbortedException($"The HTTP/2 stream was reset by the application with error code {(Http2ErrorCode)errorCode}.");
            ResetAndAbort(abortReason, (Http2ErrorCode)errorCode);
        }
    }
}
