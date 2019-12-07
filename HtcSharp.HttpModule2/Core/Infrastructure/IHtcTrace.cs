using System;
using System.Collections.Generic;
using System.Text;
using HtcSharp.Core.Logging;
using HtcSharp.HttpModule2.Connection.Exceptions;
using HtcSharp.HttpModule2.Core.Http2;
using HtcSharp.HttpModule2.Core.Http2.Frame;

namespace HtcSharp.HttpModule2.Core.Infrastructure {
    internal abstract class KestrelTrace : Logger {
        public abstract void ConnectionAccepted(string connectionId);

        public abstract void ConnectionStart(string connectionId);

        public abstract void ConnectionStop(string connectionId);

        public abstract void ConnectionPause(string connectionId);

        public abstract void ConnectionResume(string connectionId);

        public abstract  void ConnectionRejected(string connectionId);

        public abstract void ConnectionKeepAlive(string connectionId);

        public abstract void ConnectionDisconnect(string connectionId);

        public abstract void RequestProcessingError(string connectionId, Exception ex);

        public abstract void ConnectionHeadResponseBodyWrite(string connectionId, long count);

        public abstract void NotAllConnectionsClosedGracefully();

        public abstract  void ConnectionBadRequest(string connectionId, BadHttpRequestException ex);

        public abstract void ApplicationError(string connectionId, string traceIdentifier, Exception ex);

        public abstract void NotAllConnectionsAborted();

        public abstract void HeartbeatSlow(TimeSpan interval, DateTimeOffset now);

        public abstract void ApplicationNeverCompleted(string connectionId);

        public abstract void RequestBodyStart(string connectionId, string traceIdentifier);

        public abstract void RequestBodyDone(string connectionId, string traceIdentifier);

        public abstract void RequestBodyNotEntirelyRead(string connectionId, string traceIdentifier);

        public abstract void RequestBodyDrainTimedOut(string connectionId, string traceIdentifier);

        public abstract void RequestBodyMinimumDataRateNotSatisfied(string connectionId, string traceIdentifier, double rate);

        public abstract void ResponseMinimumDataRateNotSatisfied(string connectionId, string traceIdentifier);

        public abstract void ApplicationAbortedConnection(string connectionId, string traceIdentifier);

        public abstract void Http2ConnectionError(string connectionId, Http2ConnectionErrorException ex);

        public abstract void Http2ConnectionClosing(string connectionId);

        public abstract void Http2ConnectionClosed(string connectionId, int highestOpenedStreamId);

        public abstract void Http2StreamError(string connectionId, Http2StreamErrorException ex);

        public abstract void Http2StreamResetAbort(string traceIdentifier, Http2ErrorCode error, ConnectionAbortedException abortReason);

        public abstract void HPackDecodingError(string connectionId, int streamId, HPackDecodingException ex);

        public abstract void HPackEncodingError(string connectionId, int streamId, HPackEncodingException ex);

        public abstract void Http2FrameReceived(string connectionId, Http2Frame frame);

        public abstract void Http2FrameSending(string connectionId, Http2Frame frame);

        protected KestrelTrace(Type type) : base(type) { }
    }
}