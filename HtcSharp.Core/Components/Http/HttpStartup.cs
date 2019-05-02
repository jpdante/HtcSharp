using System;
using System.Diagnostics;
using System.Threading.Tasks;
using HtcSharp.Core.Engines;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Models.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HtcSharp.Core.Components.Http {
    public class HttpStartup {

        public IConfiguration Configuration { get; }
        public HttpEngine Engine;

        public HttpStartup(IConfiguration configuration, HttpEngine engine) {
            Configuration = configuration;
            Engine = engine;
        }

        public void ConfigureServices(IServiceCollection services) {
            
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            app.Run(Request);
        }

        private async Task Request(HttpContext context) {
            if (Engine.DomainServers.ContainsKey(context.Request.Host.ToString())) {
                var serverInfo = Engine.DomainServers[context.Request.Host.ToString()];
                UrlMapper.ProcessRequest(new HtcHttpContext(context), serverInfo);
            } else {
                await context.Response.WriteAsync("Unknown domain");
            }
        }
    }
}