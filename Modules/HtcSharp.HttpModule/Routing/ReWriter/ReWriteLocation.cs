using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule.Routing.ReWriter {
    public class ReWriteLocation {
        private readonly byte _actionType;
        private readonly string _data;
        private readonly List<ReWriteRule> _rules;

        public ReWriteLocation(string location, IEnumerable<string> rules) {
            _rules = new List<ReWriteRule>();
            var locationParts = location.Split(" ");
            if (locationParts[0].Equals("location", StringComparison.CurrentCultureIgnoreCase)) {
                switch (locationParts.Length) {
                    case 2:
                        _actionType = 1;
                        _data = locationParts[1];
                        break;
                    case 3: {
                        if (locationParts.Equals("=")) _actionType = 2;
                        if (locationParts.Equals("=i")) _actionType = 3;
                        if (locationParts.Equals("c")) _actionType = 4;
                        if (locationParts.Equals("ci")) _actionType = 5;
                        _data = locationParts[2];
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(locationParts));
                }
            }

            foreach (var rule in rules) {
                _rules.Add(new ReWriteRule(rule));
            }
        }

        public byte MatchRules(string request, HttpContext context, out string newRequest) {
            if (_actionType == 1) {
                var remotePath = request.Equals("/") ? "/" : $"{Path.GetDirectoryName(request).Replace(@"\", @"/")}/".Replace(@"//", @"/");
                if (_data.Equals(remotePath)) {
                    foreach (var rule in _rules) {
                        var response = rule.MatchRule(request, context, out request);
                        if (response == 1) {
                            newRequest = request;
                            return 0;
                        } else if (response == 2) {
                            context.Response.StatusCode = 302;
                            context.Response.Headers.Add("Location", request);
                            newRequest = request;
                            return 1;
                        } else if (response == 3) {
                            context.Response.StatusCode = 301;
                            context.Response.Headers.Add("Location", request);
                            newRequest = request;
                            return 1;
                        }
                    }
                }
            }

            newRequest = request;
            return 0;
        }
    }
}