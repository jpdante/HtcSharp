using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtcSharp.Core.Old.Components.Http;
using HtcSharp.Core.Old.Interfaces.Http;
using HtcSharp.Core.Old.Models.Http.Directives;
using Newtonsoft.Json.Linq;

namespace HtcSharp.Core.Old.Models.Http {
    public class LocationConfig {

        public readonly string Key = "";
        private readonly byte _modifier;
        private readonly Regex _matchRegex;
        private readonly List<IDirective> _directives;
        private readonly bool _isDefault;
        private readonly HttpLocationManager _httpLocationManager;

        public LocationConfig(string key, JArray configItems, HttpLocationManager httpLocationManager, bool isDefault = false) {
            _httpLocationManager = httpLocationManager;
            _isDefault = isDefault;
            if (!isDefault) {
                var keyData = key.Split(" ");
                if (keyData[0].Equals("location", StringComparison.CurrentCultureIgnoreCase)) {
                    if (keyData.Length == 2) {
                        if (keyData[1][0].Equals('@')) {
                            Key = keyData[1];
                        }
                        _modifier = 0;
                        _matchRegex = new Regex(keyData[1]);
                    } else if (keyData.Length == 3) {
                        switch (keyData[1]) {
                            case "=":
                                _modifier = 1;
                                _matchRegex = new Regex(keyData[2]);
                                break;
                            case "~":
                                _modifier = 2;
                                _matchRegex = new Regex(keyData[2]);
                                break;
                            case "~*":
                                _modifier = 3;
                                _matchRegex = new Regex(keyData[2], RegexOptions.IgnoreCase);
                                break;
                            case "^~":
                                _modifier = 4;
                                _matchRegex = new Regex(keyData[2]);
                                break;
                            default:
                                _modifier = 0;
                                _matchRegex = new Regex(keyData[2]);
                                break;
                        }
                    }
                }
            }
            _directives = new List<IDirective>();
            foreach (var i in configItems) {
                var dataSplit = i.ToObject<string>().Split(" ");
                switch (dataSplit[0]) {
                    case "index":
                        _directives.Add(new IndexDirective(dataSplit));
                        break;
                    case "try_files":
                        _directives.Add(new TryFilesDirective(dataSplit, _httpLocationManager));
                        break;
                    case "rewrite":
                        _directives.Add(new ReWriteDirective(dataSplit));
                        break;
                    case "try_pages":
                        _directives.Add(new TryPagesDirective(dataSplit, _httpLocationManager));
                        break;
                    case "return":
                        _directives.Add(new ReturnDirective(dataSplit));
                        break;
                    case "autoindex":
                        //_directives.Add(new LocationDirective(i));
                        break;
                    case "add_header":
                        _directives.Add(new AddHeaderDirective(dataSplit));
                        break;
                }
            }
        }

        public bool MatchLocation(HtcHttpContext context) {
            if (_isDefault) return true;
            switch (_modifier) {
                case 4:
                    return !_matchRegex.IsMatch(context.Request.Path);
                default:
                    return _matchRegex.IsMatch(context.Request.Path);
            }
        }

        public void Execute(HtcHttpContext context) {
            foreach (var directive in _directives) {
                if (!context.Response.HasStarted) directive.Execute(context);
            }
        }

    }
}
