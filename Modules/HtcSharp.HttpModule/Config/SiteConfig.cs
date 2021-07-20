﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace HtcSharp.HttpModule.Config {
    public class SiteConfig {

        public List<string> Hosts { get; set; }

        public List<string> Domains { get; set; }

        public string Root { get; set; }

        public List<LocationConfig> Locations { get; set; }

        public SslConfig SslConfig { get; set; }

        public static SiteConfig ParseConfig(JsonElement jsonElement) {
            var site = new SiteConfig();
            foreach (var property in jsonElement.EnumerateObject()) {
                string propertyName = property.Name.ToLower();
                if (propertyName.Equals("hosts")) {
                    foreach (var element in property.Value.EnumerateArray()) {
                        string elementValue = element.GetString();
                        if (string.IsNullOrEmpty(elementValue)) continue;
                        site.Hosts.Add(element.GetString());
                    }
                } else if (propertyName.Equals("domains")) {
                    foreach (var element in property.Value.EnumerateArray()) {
                        string elementValue = element.GetString();
                        if (string.IsNullOrEmpty(elementValue)) continue;
                        site.Domains.Add(element.GetString());
                    }
                    break;
                } else if (propertyName.StartsWith("location ")) {
                    propertyName = propertyName.Remove(0, 9);
                    var locationConfig = new LocationConfig(propertyName, property.Value);
                    site.Locations.Add(locationConfig);
                }
            }
            return site;
        }

    }
}