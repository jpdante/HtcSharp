using HTCSharp.Core.Components.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Engines {
    public class HttpEngine : Engine {
        private IWebHost webHost;

        public override void Start() {
            var config = EngineConfig;
            webHost = CreateWebHost(new string[] { }).Build();
            webHost.Start();
        }

        public override void Stop() {
            webHost.StopAsync().GetAwaiter().GetResult();
        }

        public IWebHostBuilder CreateWebHost(string[] args) {
            //HttpEngineContainer container = new HttpEngineContainer(typeof(HttpEngineContainer), typeof(HttpEngineContainer), ServiceLifetime.Singleton);
            IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args);
            //builder.ConfigureServices(s => { s.Add(container); });
            builder.UseUrls("http://0.0.0.0:8080");
            builder.UseStartup<HttpStartup>();
            return builder;
        }
    }
}
