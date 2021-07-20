using System.Text.Json.Serialization;

namespace HtcSharp.HttpModule.Config {
    public class HttpModuleConfig {
        
        [JsonPropertyName("SitesPath")]
        public string SitesPath { get; set; } = "sites/";

    }
}