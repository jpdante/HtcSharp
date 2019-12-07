using System;
using System.Collections.Generic;
using System.Text;
using HtcSharp.Core.Logging;

namespace HtcSharp.HttpModule2.Core.Logging {
    internal class HtcLogger {

        private readonly Logger _logger;

        public HtcLogger(Logger logger) {
            _logger = logger;
        }

        /*public virtual void ConnectionAccepted(string connectionId) {
            _connectionAccepted(_logger, connectionId, null);
        }

        public virtual void ConnectionStart(string connectionId) {
            _connectionStart(_logger, connectionId, null);
        }

        public virtual void ConnectionStop(string connectionId) {
            _connectionStop(_logger, connectionId, null);
        }

        public virtual void ConnectionPause(string connectionId) {
            _connectionPause(_logger, connectionId, null);
        }

        public virtual void ConnectionResume(string connectionId) {
            _connectionResume(_logger, connectionId, null);
        }

        public virtual void ConnectionKeepAlive(string connectionId) {
            _connectionKeepAlive(_logger, connectionId, null);
        }

        public virtual void ConnectionRejected(string connectionId) {
            _connectionRejected(_logger, connectionId, null);
        }

        public virtual void ConnectionDisconnect(string connectionId) {
            _connectionDisconnect(_logger, connectionId, null);
        }

        public virtual void ApplicationError(string connectionId, string traceIdentifier, Exception ex) {
            _applicationError(_logger, connectionId, traceIdentifier, ex);
        }

        public virtual void ConnectionHeadResponseBodyWrite(string connectionId, long count) {
            _connectionHeadResponseBodyWrite(_logger, connectionId, count, null);
        }

        public virtual void NotAllConnectionsClosedGracefully() {
            _notAllConnectionsClosedGracefully(_logger, null);
        }

        public virtual void ConnectionBadRequest(string connectionId, BadHttpRequestException ex) {
            _connectionBadRequest(_logger, connectionId, ex.Message, ex);
        }

        public virtual void RequestProcessingError(string connectionId, Exception ex) {
            _requestProcessingError(_logger, connectionId, ex);
        }

        public virtual void NotAllConnectionsAborted() {
            _notAllConnectionsAborted(_logger, null);
        }

        public virtual void HeartbeatSlow(TimeSpan interval, DateTimeOffset now) {
            _heartbeatSlow(_logger, interval, now, null);
        }

        public virtual void ApplicationNeverCompleted(string connectionId) {
            _applicationNeverCompleted(_logger, connectionId, null);
        }

        public virtual void RequestBodyStart(string connectionId, string traceIdentifier) {
            _requestBodyStart(_logger, connectionId, traceIdentifier, null);
        }

        public virtual void RequestBodyDone(string connectionId, string traceIdentifier) {
            _requestBodyDone(_logger, connectionId, traceIdentifier, null);
        }

        public virtual void RequestBodyMinimumDataRateNotSatisfied(string connectionId, string traceIdentifier, double rate) {
            _requestBodyMinimumDataRateNotSatisfied(_logger, connectionId, traceIdentifier, rate, null);
        }

        public virtual void RequestBodyNotEntirelyRead(string connectionId, string traceIdentifier) {
            _requestBodyNotEntirelyRead(_logger, connectionId, traceIdentifier, null);
        }

        public virtual void RequestBodyDrainTimedOut(string connectionId, string traceIdentifier) {
            _requestBodyDrainTimedOut(_logger, connectionId, traceIdentifier, null);
        }

        public virtual void ResponseMinimumDataRateNotSatisfied(string connectionId, string traceIdentifier) {
            _responseMinimumDataRateNotSatisfied(_logger, connectionId, traceIdentifier, null);
        }

        public virtual void ApplicationAbortedConnection(string connectionId, string traceIdentifier) {
            _applicationAbortedConnection(_logger, connectionId, traceIdentifier, null);
        }

        public virtual void Http2ConnectionError(string connectionId, Http2ConnectionErrorException ex) {
            _http2ConnectionError(_logger, connectionId, ex);
        }

        public virtual void Http2ConnectionClosing(string connectionId) {
            _http2ConnectionClosing(_logger, connectionId, null);
        }

        public virtual void Http2ConnectionClosed(string connectionId, int highestOpenedStreamId) {
            _http2ConnectionClosed(_logger, connectionId, highestOpenedStreamId, null);
        }

        public virtual void Http2StreamError(string connectionId, Http2StreamErrorException ex) {
            _http2StreamError(_logger, connectionId, ex);
        }

        public void Http2StreamResetAbort(string traceIdentifier, Http2ErrorCode error, ConnectionAbortedException abortReason) {
            _http2StreamResetError(_logger, traceIdentifier, error, abortReason);
        }

        public virtual void HPackDecodingError(string connectionId, int streamId, HPackDecodingException ex) {
            _hpackDecodingError(_logger, connectionId, streamId, ex);
        }

        public virtual void HPackEncodingError(string connectionId, int streamId, HPackEncodingException ex) {
            _hpackEncodingError(_logger, connectionId, streamId, ex);
        }

        public void Http2FrameReceived(string connectionId, Http2Frame frame) {
            _http2FrameReceived(_logger, connectionId, frame.Type, frame.StreamId, frame.PayloadLength, frame.ShowFlags(), null);
        }

        public void Http2FrameSending(string connectionId, Http2Frame frame) {
            _http2FrameSending(_logger, connectionId, frame.Type, frame.StreamId, frame.PayloadLength, frame.ShowFlags(), null);
        }*/

    }
}
