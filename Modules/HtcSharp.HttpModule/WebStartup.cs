using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HtcSharp.HttpModule {
    public class WebStartup {

        public void ConfigureServices(IServiceCollection services) {

        }

        public void Configure(IApplicationBuilder app) {
            app.Use(OnRequest);
        }

        public async Task OnRequest(HttpContext context, Func<Task> task) {

        }

    }
}