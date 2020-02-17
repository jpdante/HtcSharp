using System;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Connections.Abstractions;
using HtcSharp.HttpModule.Net.Socket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TestHtcModule {
    class Program {
        static void Main(string[] args) {
            var optionFactory = new OptionsBuilder<SocketTransportOptions>();
			optionFactory.Configure().
            var socketListener = new SocketTransportFactory();
            var loggerFactory = new LoggerFactory();
            var kestrelServer = new KestrelServer(, socketListener, loggerFactory);
        }
    }
}
