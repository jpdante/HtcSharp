using HTCSharp.Core.Logging;
using HTCSharp.Core.Models.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace HTCSharp.Core.Models.Rewriter {
    public class RewriteLocation {
        private static readonly ILog _Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly byte ActionType;
        private readonly string Data;
        private readonly List<RewriteRule> Rules;

        public RewriteLocation(string location, List<string> rules) {
            Rules = new List<RewriteRule>();
            string[] locationParts = location.Split(" ");
            if(locationParts[0].Equals("location", StringComparison.CurrentCultureIgnoreCase)) {
                if (locationParts.Length == 2) {
                    ActionType = 1;
                    Data = locationParts[1];
                } else if (locationParts.Length == 3) {
                    if (locationParts.Equals("=")) ActionType = 2;
                    if (locationParts.Equals("=i")) ActionType = 3;
                    if (locationParts.Equals("c")) ActionType = 4;
                    if (locationParts.Equals("ci")) ActionType = 5;
                    Data = locationParts[2];
                }
            }
            foreach (string rule in rules) {
                Rules.Add(new RewriteRule(rule));
            }
        }

        public byte MatchRules(string request, HTCHttpContext context, out string newRequest) {
            if (ActionType == 1) {
                string remotePath = string.Empty;
                if (request.Equals("/")) remotePath = "/";
                else remotePath = $"{Path.GetDirectoryName(request).Replace(@"\", @"/")}/".Replace(@"//", @"/");
                if (Data.Equals(remotePath)) {
                    foreach (RewriteRule rule in Rules) {
                        byte response = rule.MatchRule(request, context, out request);
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
