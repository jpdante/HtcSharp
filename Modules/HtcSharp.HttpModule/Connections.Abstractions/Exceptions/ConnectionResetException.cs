using System;
using System.IO;

namespace HtcSharp.HttpModule.Connections.Abstractions.Exceptions {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Connections.Abstractions\src\Exceptions\ConnectionResetException.cs
    // Start-At-Remote-Line 8
    // SourceTools-End
    public class ConnectionResetException : IOException {
        public ConnectionResetException(string message) : base(message) {
        }

        public ConnectionResetException(string message, Exception inner) : base(message, inner) {
        }
    }
}