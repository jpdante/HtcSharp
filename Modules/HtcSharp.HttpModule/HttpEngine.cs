using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace HtcSharp.HttpModule {
    public class HttpEngine {

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