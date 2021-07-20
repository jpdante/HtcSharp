using System.Collections.Generic;
using System.Text.Json;
using HtcSharp.HttpModule.Abstractions.Routing;

namespace HtcSharp.HttpModule.Config {
    public class LocationConfig {

        public LocationType LocationType { get; set; }

        public string Location { get; set; }

        public List<string> Actions { get; set; }

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
        }
    }
}