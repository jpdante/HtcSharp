using HTCSharp.Core.Models.Http;
using HTCSharp.Core.Models.Rewriter;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Components.Http {
    public class HttpRewriter {

        private readonly List<RewriteLocation> _rewriteLocations;

        public HttpRewriter(JObject rewritesConfig) {
            _rewriteLocations = new List<RewriteLocation>();
            foreach(var config in rewritesConfig) {
                var rules = new List<string>();
                var jArray = config.Value.Value<JArray>();
                foreach (var obj in jArray) {
                    rules.Add(obj.Value<string>());
                }
                _rewriteLocations.Add(new RewriteLocation(config.Key, rules));
            }
        }

        public bool Rewrite(HTCHttpContext context, out string outRequest) {
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
