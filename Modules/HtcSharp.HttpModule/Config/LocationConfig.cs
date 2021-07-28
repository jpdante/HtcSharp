using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using HtcSharp.HttpModule.Abstractions.Routing;

namespace HtcSharp.HttpModule.Config {
    public class LocationConfig {

        public LocationType LocationType { get; set; }

        public string Location { get; set; }

        public List<JsonElement> Directives { get; set; }

        public LocationConfig() {
            LocationType = LocationType.None;
            Location = "/";
            Directives = new List<JsonElement>();
        }

        public LocationConfig(string name, JsonElement data) {
            string[] names = name.Split(" ", 2);
            switch (names[0]) {
                case "=":
                    LocationType = LocationType.Equal;
                    Location = name;
                    break;
                case "~":
                    LocationType = LocationType.Regex;
                    Location = name;
                    break;
                case "~*":
                    LocationType = LocationType.RegexCaseInsensitive;
                    Location = name;
                    break;
                case "^~":
                    LocationType = LocationType.NoRegex;
                    Location = name;
                    break;
                default:
                    LocationType = LocationType.None;
                    Location = name;
                    break;
            }
            Directives = data.EnumerateArray().ToList();
        }
    }
}