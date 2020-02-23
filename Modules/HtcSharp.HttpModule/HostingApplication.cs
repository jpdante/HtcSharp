using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Features.Interfaces;
using HtcSharp.HttpModule.Server.Abstractions;
using Microsoft.Extensions.Logging;

namespace HtcSharp.HttpModule {
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

            var httpContext = _httpContextFactory.Create(contextFeatures);
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
