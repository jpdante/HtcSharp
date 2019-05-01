using HtcSharp.Core.Models.Http;
using HtcSharp.Core.Models.ReWriter;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core.Components.Http {
    public class HttpReWriter {

        private readonly List<ReWriteLocation> _rewriteLocations;

        public HttpReWriter(JObject rewritesConfig) {
            _rewriteLocations = new List<ReWriteLocation>();
            foreach(var config in rewritesConfig) {
                var rules = new List<string>();
                var jArray = config.Value.Value<JArray>();
                foreach (var obj in jArray) {
                    rules.Add(obj.Value<string>());
                }
                _rewriteLocations.Add(new ReWriteLocation(config.Key, rules));
            }
        }

        public bool Rewrite(HtcHttpContext context, out string outRequest) {
            var request = context.Request.Path.ToString();
            foreach (var rewriteLocation in _rewriteLocations) {
                var response = rewriteLocation.MatchRules(request, context, out request);
                outRequest = request;
                if (response == 1) return true;
            }
            outRequest = request;
            return false;
        }
    }
}
