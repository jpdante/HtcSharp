using System;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Connections.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TestHtcModule {
    class Program {
        static void Main(string[] args) {
            var loggerFactory = new LoggerFactory();
            var kestrelServer = new KestrelServer(, connectionBuilder, loggerFactory);
        }
    }
}
