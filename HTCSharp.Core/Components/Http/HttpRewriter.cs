using HTCSharp.Core.Models.Http;
using HTCSharp.Core.Models.Rewriter;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Components.Http {
    public class HttpRewriter {

        private List<RewriteLocation> RewriteLocations;

        public HttpRewriter(JObject rewritesConfig) {
            RewriteLocations = new List<RewriteLocation>();
            foreach(var config in rewritesConfig) {
                List<string> rules = new List<string>();
                JArray jArray = config.Value.Value<JArray>();
                foreach (var obj in jArray) {
                    rules.Add(obj.Value<string>());
                }
                RewriteLocations.Add(new RewriteLocation(config.Key, rules));
            }
        }

        public bool Rewrite(HTCHttpContext context, out string outRequest) {
            string request = context.Request.Path.ToString();
            foreach (RewriteLocation rewriteLocation in RewriteLocations) {
                byte response = rewriteLocation.MatchRules(request, context, out request);
                outRequest = request;
                if (response == 1) return true;
            }
            outRequest = request;
            return false;
        }
    }
}
