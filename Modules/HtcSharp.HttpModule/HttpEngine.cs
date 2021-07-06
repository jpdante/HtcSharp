using System.Threading.Tasks;
using HtcSharp.Core.Engine.Abstractions;
using HtcSharp.Core.Logging.Abstractions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Newtonsoft.Json.Linq;

namespace HtcSharp.HttpModule {
    public class HttpEngine : IEngine {

        public string Name => "Http";

        private KestrelServer _kestrelServer;

        public async Task Load(JObject config, ILogger logger) {
        }

        public async Task Start() {
    
        }

        public async Task Stop() {
     
        }
    }
}