using System.Collections.Generic;
using System.Text.Json;

namespace HtcSharp.HttpModule.Config {
    public class SiteConfig {

        public List<string> Hosts { get; set; }

        public List<string> Domains { get; set; }

        public List<string> Actions { get; set; }

        public static SiteConfig ParseConfig(JsonElement jsonElement) {
            return new SiteConfig();
        }

    }
}