using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace HtcSharp.HttpModule.Config {
    public class SiteConfig {

        public List<string> Hosts { get; set; }

        public List<string> Domains { get; set; }

        public string RootDirectory { get; set; }

        public List<LocationConfig> Locations { get; set; }

        public bool NoDelay { get; set; }

        public List<SslConfig> SslConfigs { get; set; }

        private SiteConfig() {
            Hosts = new List<string>();
            Domains = new List<string>();
            Locations = new List<LocationConfig>();
            SslConfigs = new List<SslConfig>();
            NoDelay = true;
            RootDirectory = null;
        }

        public static SiteConfig ParseConfig(JsonElement jsonElement) {
            var site = new SiteConfig();
            foreach (var property in jsonElement.EnumerateObject()) {
                string propertyName = property.Name.ToLower();
                switch (propertyName) {
                    case "hosts": {
                        foreach (var element in property.Value.EnumerateArray()) {
                            string elementValue = element.GetString();
                            if (string.IsNullOrEmpty(elementValue)) continue;
                            site.Hosts.Add(element.GetString());
                        }
                        break;
                    }
                    case "domains": {
                        foreach (var element in property.Value.EnumerateArray()) {
                            string elementValue = element.GetString();
                            if (string.IsNullOrEmpty(elementValue)) continue;
                            site.Domains.Add(element.GetString());
                        }
                        break;
                    }
                    case "ssl": {
                        foreach (var element in property.Value.EnumerateArray()) {
                            site.SslConfigs.Add(SslConfig.ParseConfig(property.Value));
                        }
                        break;
                    }
                    case "no-delay": {
                        site.NoDelay = property.Value.GetBoolean();
                        break;
                    }
                    case "root": {
                        site.RootDirectory = property.Value.GetString();
                        break;
                    }
                    default: {
                        if (propertyName.StartsWith("location ")) {
                            string name = property.Name.Remove(0, 9);
                            var locationConfig = new LocationConfig(name, property.Value);
                            site.Locations.Add(locationConfig);
                        }

                        break;
                    }
                }
            }
            if (site.Locations.Count == 0) {
                site.Locations.Add(new LocationConfig());
            }
            return site;
        }

    }
}