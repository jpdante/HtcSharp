using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.HttpModule2.Core.Infrastructure {
    internal static class Constants {
        public const int MaxExceptionDetailSize = 128;

        public static readonly string DefaultServerAddress = "http://localhost:5000";

        public static readonly string DefaultServerHttpsAddress = "https://localhost:5001";

        public const string UnixPipeHostPrefix = "unix:/";

        public const string PipeDescriptorPrefix = "pipefd:";

        public const string SocketDescriptorPrefix = "sockfd:";

        public const string ServerName = "Kestrel";

        public static readonly TimeSpan RequestBodyDrainTimeout = TimeSpan.FromSeconds(5);
    }
}