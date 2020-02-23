using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.Core.Engine.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Default;
using HtcSharp.HttpModule.Logging;
using HtcSharp.HttpModule.Net.Socket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace HtcSharp.HttpModule {
    public class HttpEngine : IEngine {

        public string Name => "HtcHttp";

        private Core.Logging.Abstractions.ILogger _logger;
        private IServiceProvider _serviceProvider;
        private ILoggerFactory _loggerFactory;
        private SocketTransportFactory _socketTransportFactory;
        private KestrelServer _kestrelServer;
        private CancellationToken _cancellationToken;

        public Task Load(JObject config, Core.Logging.Abstractions.ILogger logger) {
            _logger = logger;
            _serviceProvider = SetupServiceCollection().BuildServiceProvider();
            _loggerFactory = LoggerFactory.Create(builder => {
                builder.AddProvider(new HtcLoggerProvider(_logger));
            });
            _socketTransportFactory = new SocketTransportFactory(GetSocketTransportOptions(), _loggerFactory);
            _kestrelServer = new KestrelServer(GetKestrelServerOptions(_serviceProvider), _socketTransportFactory, _loggerFactory);
            return Task.CompletedTask;
        }

        public async Task Start() {
            _cancellationToken = new CancellationToken();
            var logger = _loggerFactory.CreateLogger("HtcLogger");
            await _kestrelServer.StartAsync(new HostingApplication(logger, _serviceProvider.GetRequiredService<IHttpContextFactory>()), _cancellationToken);
        }

        public async Task Stop() {
            await _kestrelServer.StopAsync(_cancellationToken);
        }

        private ServiceCollection SetupServiceCollection() {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IHttpContextFactory, DefaultHttpContextFactory>();
            serviceCollection.AddTransient<ILogger, Logger<KestrelServer>>();
            //var listener = new DiagnosticListener("HtcSharpServer");
            //serviceCollection.AddSingleton<DiagnosticListener>(listener);
            //serviceCollection.AddSingleton<DiagnosticSource>(listener);
            serviceCollection.AddOptions();
            serviceCollection.AddLogging();
            return serviceCollection;
        }

        private static IOptions<SocketTransportOptions> GetSocketTransportOptions() {
            var socketTransportOptions = new SocketTransportOptions();
            IOptions<SocketTransportOptions> optionFactory = Microsoft.Extensions.Options.Options.Create(socketTransportOptions);
            return optionFactory;
        }

        private static IOptions<KestrelServerOptions> GetKestrelServerOptions(IServiceProvider serviceProvider) {
            var kestrelServerOptions = new KestrelServerOptions { ApplicationServices = serviceProvider };
            kestrelServerOptions.Configure().AnyIPEndpoint(8080);
            IOptions<KestrelServerOptions> optionFactory = Microsoft.Extensions.Options.Options.Create(kestrelServerOptions);
            return optionFactory;
        }
    }
}