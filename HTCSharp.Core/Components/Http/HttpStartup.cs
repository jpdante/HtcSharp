using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HTCSharp.Core.Components.Http {
    public class HttpStartup {

        public IConfiguration Configuration { get; }

        public HttpStartup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            app.Run(async context => {
                await context.Response.WriteAsync("Teste");
            });
        }
    }
}