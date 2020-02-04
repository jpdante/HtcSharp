using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Core.Http.Features;
using HtcSharp.HttpModule.Core.Infrastructure;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Http.Http.Abstractions;
using HtcSharp.HttpModule.Infrastructure.Extensions;
using HtcSharp.HttpModule.Infrastructure.Features;

namespace HtcSharp.HttpModule.Core.Http.Http {
    internal partial class HttpProtocol : IHttpRequestFeature,
                                          IHttpResponseFeature,
                                          IHttpResponseBodyFeature,
                                          IRequestBodyPipeFeature,
                                          IHttpUpgradeFeature,
                                          IHttpConnectionFeature,
                                          IHttpRequestLifetimeFeature,
                                          IHttpRequestIdentifierFeature,
                                          IHttpRequestTrailersFeature,
                                          IHttpBodyControlFeature,
                                          IHttpMaxRequestBodySizeFeature,
                                          IEndpointFeature,
                                          IRouteValuesFeature {
        // NOTE: When feature interfaces are added to or removed from this HttpProtocol class implementation,
        // then the list of `implementedFeatures` in the generated code project MUST also be updated.
        // See also: tools/CodeGenerator/HttpProtocolFeatureCollection.cs

        string IHttpRequestFeature.Protocol {
            get => HttpVersion;
            set => HttpVersion = value;
        }

        string IHttpRequestFeature.Scheme {
            get => Scheme ?? "http";
            set => Scheme = value;
        }

        string IHttpRequestFeature.Method {
            get {
                if (_methodText != null) {
                    return _methodText;
                }

                _methodText = HttpUtilities.MethodToString(Method) ?? string.Empty;
                return _methodText;
            }
            set {
                _methodText = value;
            }
        }

        string IHttpRequestFeature.PathBase {
            get => PathBase ?? "";
            set => PathBase = value;
        }

        string IHttpRequestFeature.Path {
            get => Path;
            set => Path = value;
        }

        string IHttpRequestFeature.QueryString {
            get => QueryString;
            set => QueryString = value;
        }

        string IHttpRequestFeature.RawTarget {
            get => RawTarget;
            set => RawTarget = value;
        }

        IHeaderDictionary IHttpRequestFeature.Headers {
            get => RequestHeaders;
            set => RequestHeaders = value;
        }

        Stream IHttpRequestFeature.Body {
            get => RequestBody;
            set => RequestBody = value;
        }

        PipeReader IRequestBodyPipeFeature.Reader {
            get {
                if (!ReferenceEquals(_requestStreamInternal, RequestBody)) {
                    _requestStreamInternal = RequestBody;
                    RequestBodyPipeReader = PipeReader.Create(RequestBody, new StreamPipeReaderOptions(_context.MemoryPool, _context.MemoryPool.GetMinimumSegmentSize(), _context.MemoryPool.GetMinimumAllocSize()));

                    OnCompleted((self) => {
                        ((PipeReader)self).Complete();
                        return Task.CompletedTask;
                    }, RequestBodyPipeReader);
                }

                return RequestBodyPipeReader;
            }
        }

        bool IHttpRequestTrailersFeature.Available => RequestTrailersAvailable;

        IHeaderDictionary IHttpRequestTrailersFeature.Trailers {
            get {
                if (!RequestTrailersAvailable) {
                    throw new InvalidOperationException("The request trailers are not available yet. They may not be available until the full request body is read.");
                }
                return RequestTrailers;
            }
        }

        int IHttpResponseFeature.StatusCode {
            get => StatusCode;
            set => StatusCode = value;
        }

        string IHttpResponseFeature.ReasonPhrase {
            get => ReasonPhrase;
            set => ReasonPhrase = value;
        }

        IHeaderDictionary IHttpResponseFeature.Headers {
            get => ResponseHeaders;
            set => ResponseHeaders = value;
        }

        CancellationToken IHttpRequestLifetimeFeature.RequestAborted {
            get => RequestAborted;
            set => RequestAborted = value;
        }

        bool IHttpResponseFeature.HasStarted => HasResponseStarted;

        bool IHttpUpgradeFeature.IsUpgradableRequest => IsUpgradableRequest;

        IPAddress IHttpConnectionFeature.RemoteIpAddress {
            get => RemoteIpAddress;
            set => RemoteIpAddress = value;
        }

        IPAddress IHttpConnectionFeature.LocalIpAddress {
            get => LocalIpAddress;
            set => LocalIpAddress = value;
        }

        int IHttpConnectionFeature.RemotePort {
            get => RemotePort;
            set => RemotePort = value;
        }

        int IHttpConnectionFeature.LocalPort {
            get => LocalPort;
            set => LocalPort = value;
        }

        string IHttpConnectionFeature.ConnectionId {
            get => ConnectionIdFeature;
            set => ConnectionIdFeature = value;
        }

        string IHttpRequestIdentifierFeature.TraceIdentifier {
            get => TraceIdentifier;
            set => TraceIdentifier = value;
        }

        bool IHttpBodyControlFeature.AllowSynchronousIO {
            get => AllowSynchronousIO;
            set => AllowSynchronousIO = value;
        }

        bool IHttpMaxRequestBodySizeFeature.IsReadOnly => HasStartedConsumingRequestBody || IsUpgraded;

        long? IHttpMaxRequestBodySizeFeature.MaxRequestBodySize {
            get => MaxRequestBodySize;
            set {
                if (HasStartedConsumingRequestBody) {
                    throw new InvalidOperationException("The maximum request body size cannot be modified after the app has already started reading from the request body.");
                }
                if (IsUpgraded) {
                    throw new InvalidOperationException("The maximum request body size cannot be modified after the request has been upgraded.");
                }
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be null or a non-negative number.");
                }

                MaxRequestBodySize = value;
            }
        }

        Stream IHttpResponseFeature.Body {
            get => ResponseBody;
            set => ResponseBody = value;
        }

        PipeWriter IHttpResponseBodyFeature.Writer => ResponseBodyPipeWriter;

        Endpoint IEndpointFeature.Endpoint {
            get => _endpoint;
            set => _endpoint = value;
        }

        RouteValueDictionary IRouteValuesFeature.RouteValues {
            get => _routeValues ??= new RouteValueDictionary();
            set => _routeValues = value;
        }

        Stream IHttpResponseBodyFeature.Stream => ResponseBody;

        protected void ResetHttp1Features() {
            _currentIHttpMinRequestBodyDataRateFeature = this;
            _currentIHttpMinResponseDataRateFeature = this;
        }

        protected void ResetHttp2Features() {
            _currentIHttp2StreamIdFeature = this;
            _currentIHttpResponseTrailersFeature = this;
            _currentIHttpResetFeature = this;
        }

        void IHttpResponseFeature.OnStarting(Func<object, Task> callback, object state) {
            OnStarting(callback, state);
        }

        void IHttpResponseFeature.OnCompleted(Func<object, Task> callback, object state) {
            OnCompleted(callback, state);
        }

        async Task<Stream> IHttpUpgradeFeature.UpgradeAsync() {
            if (!IsUpgradableRequest) {
                throw new InvalidOperationException("Cannot upgrade a non-upgradable request. Check IHttpUpgradeFeature.IsUpgradableRequest to determine if a request can be upgraded.");
            }

            if (IsUpgraded) {
                throw new InvalidOperationException("IHttpUpgradeFeature.UpgradeAsync was already called and can only be called once per connection.");
            }

            if (!ServiceContext.ConnectionManager.UpgradedConnectionCount.TryLockOne()) {
                throw new InvalidOperationException("Request cannot be upgraded because the server has already opened the maximum number of upgraded connections.");
            }

            IsUpgraded = true;

            ConnectionFeatures.Get<IDecrementConcurrentConnectionCountFeature>()?.ReleaseConnection();

            StatusCode = StatusCodes.Status101SwitchingProtocols;
            ReasonPhrase = "Switching Protocols";
            ResponseHeaders[HeaderNames.Connection] = "Upgrade";

            await FlushAsync();

            return _bodyControl.Upgrade();
        }

        void IHttpRequestLifetimeFeature.Abort() {
            ApplicationAbort();
        }

        Task IHttpResponseBodyFeature.StartAsync(CancellationToken cancellationToken) {
            if (HasResponseStarted) {
                return Task.CompletedTask;
            }

            cancellationToken.ThrowIfCancellationRequested();

            return InitializeResponseAsync(0);
        }

        void IHttpResponseBodyFeature.DisableBuffering() {
        }

        Task IHttpResponseBodyFeature.SendFileAsync(string path, long offset, long? count, CancellationToken cancellation) {
            return SendFileFallback.SendFileAsync(ResponseBody, path, offset, count, cancellation);
        }

        Task IHttpResponseBodyFeature.CompleteAsync() {
            return CompleteAsync();
        }
    }
}
