using System;

namespace HtcSharp.HttpModule2.Connection.Exceptions {
    public class ConnectionAbortedException : OperationCanceledException {
        public ConnectionAbortedException() : this("The connection was aborted") {
        }

        public ConnectionAbortedException(string message) : base(message) {
        }

        public ConnectionAbortedException(string message, Exception inner) : base(message, inner) {
        }
    }
}
