﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;

namespace HtcSharp.HttpModule.StaticFiles {
    internal struct StaticFileContext {
        private readonly HttpContext _context;
        private readonly HttpRequest _request;
        private readonly HttpResponse _response;
        private readonly IFileProvider _fileProvider;

        private IFileInfo _fileInfo;
        private string _contentType;
        private EntityTagHeaderValue _etag;
        private RequestHeaders _requestHeaders;
        private ResponseHeaders _responseHeaders;
        private RangeItemHeaderValue _range;

        private long _length;
        private readonly PathString _subPath;
        private DateTimeOffset _lastModified;

        private PreconditionState _ifMatchState;
        private PreconditionState _ifNoneMatchState;
        private PreconditionState _ifModifiedSinceState;
        private PreconditionState _ifUnmodifiedSinceState;

        private RequestType _requestType;

        public StaticFileContext(HttpContext context, IFileProvider fileProvider, PathString subPath) {
            _context = context;
            _request = context.Request;
            _response = context.Response;
            _fileProvider = fileProvider;
            string method = _request.Method;
            _fileInfo = null;
            _etag = null;
            _requestHeaders = null;
            _responseHeaders = null;
            _range = null;
            _contentType = ContentType.DEFAULT.ToValue();

            _length = 0;
            _subPath = subPath;
            _lastModified = new DateTimeOffset();
            _ifMatchState = PreconditionState.Unspecified;
            _ifNoneMatchState = PreconditionState.Unspecified;
            _ifModifiedSinceState = PreconditionState.Unspecified;
            _ifUnmodifiedSinceState = PreconditionState.Unspecified;

            if (HttpMethods.IsGet(method)) {
                _requestType = RequestType.IsGet;
            } else if (HttpMethods.IsHead(method)) {
                _requestType = RequestType.IsHead;
            } else {
                _requestType = RequestType.Unspecified;
            }
        }

        private RequestHeaders RequestHeaders => _requestHeaders ??= _request.GetTypedHeaders();

        private ResponseHeaders ResponseHeaders => _responseHeaders ??= _response.GetTypedHeaders();

        public bool IsHeadMethod => _requestType.HasFlag(RequestType.IsHead);

        public bool IsGetMethod => _requestType.HasFlag(RequestType.IsGet);

        public bool IsRangeRequest {
            get => _requestType.HasFlag(RequestType.IsRange);
            private set {
                if (value) {
                    _requestType |= RequestType.IsRange;
                } else {
                    _requestType &= ~RequestType.IsRange;
                }
            }
        }

        public string SubPath => _subPath.Value;

        public string PhysicalPath => _fileInfo?.PhysicalPath;

        public bool LookupFileInfo() {
            _fileInfo = _fileProvider.GetFileInfo(_subPath.Value);
            if (_fileInfo.Exists) {
                _length = _fileInfo.Length;
                _contentType = ContentType.DEFAULT.FromExtension(_fileInfo.PhysicalPath).ToValue();

                DateTimeOffset last = _fileInfo.LastModified;
                // Truncate to the second.
                _lastModified = new DateTimeOffset(last.Year, last.Month, last.Day, last.Hour, last.Minute, last.Second, last.Offset).ToUniversalTime();

                long etagHash = _lastModified.ToFileTime() ^ _length;
                _etag = new EntityTagHeaderValue('\"' + Convert.ToString(etagHash, 16) + '\"');
            }

            return _fileInfo.Exists;
        }

        public void ComprehendRequestHeaders() {
            ComputeIfMatch();

            ComputeIfModifiedSince();

            ComputeRange();

            ComputeIfRange();
        }

        private void ComputeIfMatch() {
            var requestHeaders = RequestHeaders;

            // 14.24 If-Match
            var ifMatch = requestHeaders.IfMatch;
            if (ifMatch?.Count > 0) {
                _ifMatchState = PreconditionState.PreconditionFailed;
                foreach (var etag in ifMatch) {
                    if (etag.Equals(EntityTagHeaderValue.Any) || etag.Compare(_etag, useStrongComparison: true)) {
                        _ifMatchState = PreconditionState.ShouldProcess;
                        break;
                    }
                }
            }

            // 14.26 If-None-Match
            var ifNoneMatch = requestHeaders.IfNoneMatch;
            if (ifNoneMatch?.Count > 0) {
                _ifNoneMatchState = PreconditionState.ShouldProcess;
                foreach (var etag in ifNoneMatch) {
                    if (etag.Equals(EntityTagHeaderValue.Any) || etag.Compare(_etag, useStrongComparison: true)) {
                        _ifNoneMatchState = PreconditionState.NotModified;
                        break;
                    }
                }
            }
        }

        private void ComputeIfModifiedSince() {
            var requestHeaders = RequestHeaders;
            var now = DateTimeOffset.UtcNow;

            // 14.25 If-Modified-Since
            var ifModifiedSince = requestHeaders.IfModifiedSince;
            if (ifModifiedSince.HasValue && ifModifiedSince <= now) {
                bool modified = ifModifiedSince < _lastModified;
                _ifModifiedSinceState = modified ? PreconditionState.ShouldProcess : PreconditionState.NotModified;
            }

            // 14.28 If-Unmodified-Since
            var ifUnmodifiedSince = requestHeaders.IfUnmodifiedSince;
            if (ifUnmodifiedSince.HasValue && ifUnmodifiedSince <= now) {
                bool unmodified = ifUnmodifiedSince >= _lastModified;
                _ifUnmodifiedSinceState = unmodified ? PreconditionState.ShouldProcess : PreconditionState.PreconditionFailed;
            }
        }

        private void ComputeIfRange() {
            // 14.27 If-Range
            var ifRangeHeader = RequestHeaders.IfRange;
            if (ifRangeHeader != null) {
                // If the validator given in the If-Range header field matches the
                // current validator for the selected representation of the target
                // resource, then the server SHOULD process the Range header field as
                // requested.  If the validator does not match, the server MUST ignore
                // the Range header field.
                if (ifRangeHeader.LastModified.HasValue) {
                    if (_lastModified > ifRangeHeader.LastModified) {
                        IsRangeRequest = false;
                    }
                } else if (_etag != null && ifRangeHeader.EntityTag != null && !ifRangeHeader.EntityTag.Compare(_etag, useStrongComparison: true)) {
                    IsRangeRequest = false;
                }
            }
        }

        private void ComputeRange() {
            // 14.35 Range
            // http://tools.ietf.org/html/draft-ietf-httpbis-p5-range-24

            // A server MUST ignore a Range header field received with a request method other
            // than GET.
            if (!IsGetMethod) {
                return;
            }

            (var isRangeRequest, var range) = RangeHelper.ParseRange(_context, RequestHeaders, _length);

            _range = range;
            IsRangeRequest = isRangeRequest;
        }

        public void ApplyResponseHeaders(int statusCode) {
            _response.StatusCode = statusCode;
            if (statusCode < 400) {
                // these headers are returned for 200, 206, and 304
                // they are not returned for 412 and 416
                if (!string.IsNullOrEmpty(_contentType)) {
                    _response.ContentType = _contentType;
                }

                var responseHeaders = ResponseHeaders;
                responseHeaders.LastModified = _lastModified;
                responseHeaders.ETag = _etag;
                responseHeaders.Headers[HeaderNames.AcceptRanges] = "bytes";
            }

            if (statusCode == StatusCodes.Status200OK) {
                // this header is only returned here for 200
                // it already set to the returned range for 206
                // it is not returned for 304, 412, and 416
                _response.ContentLength = _length;
            }
        }

        public PreconditionState GetPreconditionState()
            => GetMaxPreconditionState(_ifMatchState, _ifNoneMatchState, _ifModifiedSinceState, _ifUnmodifiedSinceState);

        private static PreconditionState GetMaxPreconditionState(params PreconditionState[] states) {
            PreconditionState max = PreconditionState.Unspecified;
            for (int i = 0; i < states.Length; i++) {
                if (states[i] > max) {
                    max = states[i];
                }
            }

            return max;
        }

        public Task SendStatusAsync(int statusCode) {
            ApplyResponseHeaders(statusCode);

            return Task.CompletedTask;
        }

        public async Task ServeStaticFile(HtcHttpContext context, DirectiveDelegate next) {
            ComprehendRequestHeaders();
            switch (GetPreconditionState()) {
                case PreconditionState.Unspecified:
                case PreconditionState.ShouldProcess:
                    if (IsHeadMethod) {
                        await SendStatusAsync(StatusCodes.Status200OK);
                        return;
                    }

                    try {
                        if (IsRangeRequest) {
                            await SendRangeAsync();
                            return;
                        }

                        await SendAsync();
                        return;
                    } catch (FileNotFoundException) {
                        context.Response.Clear();
                    }

                    await next(context);
                    return;
                case PreconditionState.NotModified:
                    await SendStatusAsync(StatusCodes.Status304NotModified);
                    return;
                case PreconditionState.PreconditionFailed:
                    await SendStatusAsync(StatusCodes.Status412PreconditionFailed);
                    return;
                default:
                    var exception = new NotImplementedException(GetPreconditionState().ToString());
                    Debug.Fail(exception.ToString());
                    throw exception;
            }
        }

        public async Task SendAsync() {
            ApplyResponseHeaders(StatusCodes.Status200OK);
            try {
                await _context.Response.SendFileAsync(_fileInfo, 0, _length, _context.RequestAborted);
            } catch (OperationCanceledException) {
                // Don't throw this exception, it's most likely caused by the client disconnecting.
            }
        }

        // When there is only a single range the bytes are sent directly in the body.
        internal async Task SendRangeAsync() {
            if (_range == null) {
                // 14.16 Content-Range - A server sending a response with status code 416 (Requested range not satisfiable)
                // SHOULD include a Content-Range field with a byte-range-resp-spec of "*". The instance-length specifies
                // the current length of the selected resource.  e.g. #1#length
                ResponseHeaders.ContentRange = new ContentRangeHeaderValue(_length);
                ApplyResponseHeaders(StatusCodes.Status416RangeNotSatisfiable);
                return;
            }

            ResponseHeaders.ContentRange = ComputeContentRange(_range, out var start, out var length);
            _response.ContentLength = length;
            ApplyResponseHeaders(StatusCodes.Status206PartialContent);

            try {
                await _context.Response.SendFileAsync(_fileInfo, start, length, _context.RequestAborted);
            } catch (OperationCanceledException) {
                // Don't throw this exception, it's most likely caused by the client disconnecting.
            }
        }

        // Note: This assumes ranges have been normalized to absolute byte offsets.
        private ContentRangeHeaderValue ComputeContentRange(RangeItemHeaderValue range, out long start, out long length) {
            start = range.From.Value;
            long end = range.To.Value;
            length = end - start + 1;
            return new ContentRangeHeaderValue(start, end, _length);
        }

        internal enum PreconditionState : byte {
            Unspecified,
            NotModified,
            ShouldProcess,
            PreconditionFailed
        }

        [Flags]
        private enum RequestType : byte {
            Unspecified = 0b_000,
            IsHead = 0b_001,
            IsGet = 0b_010,
            IsRange = 0b_100,
        }
    }
}