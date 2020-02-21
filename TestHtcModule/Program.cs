using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Connections.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Default;
using HtcSharp.HttpModule.Http.Features;
using HtcSharp.HttpModule.Http.Features.Interfaces;
using HtcSharp.HttpModule.Net.Socket;
using HtcSharp.HttpModule.Server.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TestHtcModule {
    public class Program {
        public static void Main(string[] args) {
            new Program().Run();
        }

        public static IOptions<SocketTransportOptions> GetSocketTransportOptions() {
            var socketTransportOptions = new SocketTransportOptions();
            var optionFactory = Options.Create<SocketTransportOptions>(socketTransportOptions);
            return optionFactory;
        }

        public static IOptions<KestrelServerOptions> GetKestrelServerOptions(IServiceProvider serviceProvider) {
            var kestrelServerOptions = new KestrelServerOptions {ApplicationServices = serviceProvider};
            kestrelServerOptions.Configure().AnyIPEndpoint(8080);
            var optionFactory = Options.Create<KestrelServerOptions>(kestrelServerOptions);
            return optionFactory;
        }



        public void Run() {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IHttpContextFactory, DefaultHttpContextFactory>();
            serviceCollection.AddTransient<ILogger, Logger<KestrelServer>>();
            var listener = new DiagnosticListener("HtcSharpServer");
            serviceCollection.AddSingleton<DiagnosticListener>(listener);
            serviceCollection.AddSingleton<DiagnosticSource>(listener);
            serviceCollection.AddOptions();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var loggerFactory = LoggerFactory.Create(DoSomething);
            var socketListener = new SocketTransportFactory(GetSocketTransportOptions(), loggerFactory);
            var kestrelServer = new KestrelServer(GetKestrelServerOptions(serviceProvider), socketListener, loggerFactory);
            while (true) {
                Console.Write(@"> ");
                var msg = Console.ReadLine();
                if (msg == null) continue;
                if (msg.Equals("exit", StringComparison.CurrentCultureIgnoreCase)) {
                    kestrelServer.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
                    return;
                } else if (msg.Equals("start", StringComparison.CurrentCultureIgnoreCase)) {
                    Start(kestrelServer, serviceProvider, loggerFactory);
                }
            }
        }

        private void DoSomething(ILoggingBuilder obj) {
            obj.AddProvider(new ConsoleLoggerProvider());
        }

        public async void Start(KestrelServer kestrelServer, IServiceProvider serviceProvider, ILoggerFactory loggerFactory) {
            var logger = loggerFactory.CreateLogger("ConsoleRequester");
            await kestrelServer.StartAsync(new HostingApplication(logger, serviceProvider.GetRequiredService<IHttpContextFactory>()), CancellationToken.None);
        }
    }

    internal class HostingApplication : IHttpApplication<HostingApplication.Context> {
        private readonly IHttpContextFactory _httpContextFactory;
        private readonly ILogger _logger;

        public HostingApplication(ILogger logger, IHttpContextFactory httpContextFactory) {
            _httpContextFactory = httpContextFactory;
            _logger = logger;
        }

        public Context CreateContext(IFeatureCollection contextFeatures) {
            Context hostContext;
            if (contextFeatures is IHostContextContainer<Context> container) {
                hostContext = container.HostContext;
                if (hostContext is null) {
                    hostContext = new Context();
                    container.HostContext = hostContext;
                }
            } else {
                hostContext = new Context();
            }

            HttpContext httpContext = _httpContextFactory.Create(contextFeatures);
            hostContext.HttpContext = httpContext;

            return hostContext;
        }

        public async Task ProcessRequestAsync(Context context) {
            var response = context.HttpContext.Response;
            response.StatusCode = 200;
            Memory<byte> data = Encoding.UTF8.GetBytes("AAAA");
            await response.BodyWriter.WriteAsync(data);
        }

        public void DisposeContext(Context context, Exception exception) {
            var httpContext = context.HttpContext;

            _httpContextFactory.Dispose(httpContext);

            context.Reset();
        }


        internal class Context {
            public HttpContext HttpContext { get; set; }
            public IDisposable Scope { get; set; }

            public long StartTimestamp { get; set; }
            internal bool HasDiagnosticListener { get; set; }
            public bool EventLogEnabled { get; set; }

            public void Reset() {
                Scope = null;
                StartTimestamp = 0;
                HasDiagnosticListener = false;
                EventLogEnabled = false;
            }
        }
    }
}
