// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Features;
using HtcSharp.HttpModule.Server.Abstractions;
using Microsoft.Extensions.Logging;

namespace HtcSharp.HttpModule.Hosting.Internal {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Hosting\Hosting\src\Internal\HostingApplication.cs
    // Start-At-Remote-Line 14
    // Ignore-Local-Line-Range 19-100
    // SourceTools-End
    internal class HostingApplication : IHttpApplication<HostingApplication.Context> {
        private readonly HttpEngine _httpEngine;
        private readonly ILogger _logger;
        private readonly IHttpContextFactory _httpContextFactory;

        public HostingApplication(HttpEngine httpEngine, ILogger logger, IHttpContextFactory httpContextFactory) {
            _httpEngine = httpEngine;
            _logger = logger;
            _httpContextFactory = httpContextFactory;
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
            try {
                // Https = 1
                // Http = 0
                string currentKey = context.HttpContext.Request.IsHttps
                    ? $"1{context.HttpContext.Request.Host}"
                    : $"0{context.HttpContext.Request.Host}";
                if (_httpEngine.DomainDictionary.TryGetValue(currentKey, out var value)) {
                    context.HttpContext.ServerInfo = value.HttpServerInfo;
                    await value.LocationManager.ProcessRequest(context.HttpContext);
                    return;
                }

                string anyKey = context.HttpContext.Request.IsHttps ? $"1*" : $"0*";
                if (_httpEngine.DomainDictionary.TryGetValue(anyKey, out var value2)) {
                    context.HttpContext.ServerInfo = value2.HttpServerInfo;
                    await value2.LocationManager.ProcessRequest(context.HttpContext);
                    return;
                }

                var response = context.HttpContext.Response;
                response.StatusCode = 503;
                await response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes("No domain found!"));
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }
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