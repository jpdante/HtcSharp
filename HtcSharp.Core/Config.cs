using System.Text.Json.Serialization;

namespace HtcSharp.Core {
    public class Config {

        [JsonPropertyName("modulesPath")]
        public string ModulesPath { get; set; } = "./modules/";

        [JsonPropertyName("pluginsPath")]
        public string PluginsPath { get; set; } = "./plugins/";

    }
}