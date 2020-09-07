using System;

namespace HtcSharp.HttpModule.Connections.Abstractions.Exceptions {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Connections.Abstractions\src\Exceptions\ConnectionAbortedException.cs
    // Start-At-Remote-Line 4
    // SourceTools-End
    public class ConnectionAbortedException : OperationCanceledException {
        public ConnectionAbortedException() :
            this("The connection was aborted") {
        }

        public ConnectionAbortedException(string message) : base(message) {
        }

        public ConnectionAbortedException(string message, Exception inner) : base(message, inner) {
        }
    }
}