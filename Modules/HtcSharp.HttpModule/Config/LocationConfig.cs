using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HtcSharp.HttpModule.Config {
    public class LocationConfig {
        
        [JsonPropertyName("location")]
        public string Location { get; set; }

        public List<string> Actions { get; set; }
    }
}