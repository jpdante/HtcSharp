using HTCSharp.Core.Logging;
using HTCSharp.Core.Models.Http;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace HTCSharp.Core.Models.Rewriter {
    public class RewriteRule {
        private static readonly ILog _Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly byte RuleType;
        private readonly Regex Pattern;
        private readonly string RewriteData;
        private readonly string Flag;
        private readonly int StatusCode;

        public RewriteRule(string rule) {
            string[] ruleParts = rule.Split(" ");
            if(ruleParts[0].Equals("rewrite", StringComparison.CurrentCultureIgnoreCase)) {
                RuleType = 1;
                Pattern = new Regex(ruleParts[1]);
                RewriteData = ruleParts[2];
                if (ruleParts.Length == 4) Flag = ruleParts[3];
            } else if(ruleParts[0].Equals("return", StringComparison.CurrentCultureIgnoreCase)) {
                RuleType = 2;
                StatusCode = int.Parse(ruleParts[1]);
                RewriteData = ruleParts[2];
                Flag = string.Empty;
            } 
        }

        public byte MatchRule(string request, HTCHttpContext context, out string newRequest) {
            if (RuleType == 1) {
                if (Pattern.IsMatch(request)) {
                    byte response = 0;
                    string[] requestParts = request.Split("/");
                    string[] rewriteQuery = RewriteData.Split("?");
                    request = rewriteQuery[0].Replace("$scheme", context.Request.Scheme);
                    if (rewriteQuery.Length == 2) {
                        string[] queryParts = rewriteQuery[1].Split("&");
                        foreach (string query in queryParts) {
                            string[] queryData = query.Split("=");
                            string key = queryData[0];
                            string value = queryData[1];
                            for (int p = 1; p < requestParts.Length; p++) {
                                value = value.Replace($"${p}", requestParts[p]);
                            }
                            context.Request.Query.Add(key, value);
                        }
                    }
                    if (Flag.Equals("last", StringComparison.CurrentCultureIgnoreCase)) response = 0;
                    if (Flag.Equals("break", StringComparison.CurrentCultureIgnoreCase)) response = 1;
                    if (Flag.Equals("redirect", StringComparison.CurrentCultureIgnoreCase)) response = 2;
                    if (Flag.Equals("permanent", StringComparison.CurrentCultureIgnoreCase)) response = 3;
                    newRequest = request;
                    return response;
                }
            } else if (RuleType == 2) {
                newRequest = request;
                return 0;
            }
            newRequest = request;
            return 0;
        }
    }
}
