using System;
using System.Diagnostics;
using System.Threading.Tasks;
using HTCSharp.Core.Engines;
using HTCSharp.Core.Helpers.Http;
using HTCSharp.Core.Models.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HTCSharp.Core.Components.Http {
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
            if (Engine.getDomainServers.ContainsKey(context.Request.Host.ToString())) {
                var serverInfo = Engine.getDomainServers[context.Request.Host.ToString()];
                URLMapping.ProcessRequest(new HTCHttpContext(context), serverInfo);
            } else {
                await context.Response.WriteAsync("Unknown domain");
            }
        }
    }
}