using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace HtcSharp.HttpModule.Config {
    public class SiteConfig {

        public List<string> Hosts { get; set; }

        public List<string> Domains { get; set; }

        public List<LocationConfig> Locations { get; set; }

        public List<SslConfig> SslConfigs { get; set; }

        public HashSet<string> AllowedPlugins { get; set; }

        public HashSet<string> ForbiddenPlugins { get; set; }

        public bool PluginsEnabled { get; set; }

        public bool DefaultPluginPermission { get; set; }

        public string RootDirectory { get; set; }

        public bool NoDelay { get; set; }

        private SiteConfig() {
            Hosts = new List<string>();
            Domains = new List<string>();
            Locations = new List<LocationConfig>();
            SslConfigs = new List<SslConfig>();
            AllowedPlugins = new HashSet<string>();
            ForbiddenPlugins = new HashSet<string>();
            PluginsEnabled = true;
            DefaultPluginPermission = true;
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
                            site.SslConfigs.Add(SslConfig.ParseConfig(element));
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
                    case "plugins": {
                        if (property.Value.TryGetProperty("enabled", out var enabledElement) && enabledElement.ValueKind is JsonValueKind.False or JsonValueKind.True) {
                            site.PluginsEnabled = enabledElement.GetBoolean();
                        }
                        if (property.Value.TryGetProperty("default-permission", out var defaultPermissionElement) && defaultPermissionElement.ValueKind is JsonValueKind.False or JsonValueKind.True) {
                            site.DefaultPluginPermission = defaultPermissionElement.GetBoolean();
                        }
                        if (property.Value.TryGetProperty("forbidden", out var forbiddenElement)) {
                            foreach (string value in forbiddenElement.EnumerateArray().Select(element => element.GetString()).Where(value => !string.IsNullOrEmpty(value))) {
                                site.ForbiddenPlugins.Add(value);
                            }
                        }
                        if (property.Value.TryGetProperty("allowed", out var allowedElement) && allowedElement.ValueKind == JsonValueKind.Array) {
                            foreach (string value in allowedElement.EnumerateArray().Select(element => element.GetString()).Where(value => !string.IsNullOrEmpty(value))) {
                                site.AllowedPlugins.Add(value);
                            }
                        }
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