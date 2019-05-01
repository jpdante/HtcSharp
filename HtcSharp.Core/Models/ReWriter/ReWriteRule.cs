using HtcSharp.Core.Logging;
using HtcSharp.Core.Models.Http;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace HtcSharp.Core.Models.ReWriter {
    public class ReWriteRule {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly byte _ruleType;
        private readonly Regex _pattern;
        private readonly string _rewriteData;
        private readonly string _flag;
        private readonly int _statusCode;

        public ReWriteRule(string rule) {
            var ruleParts = rule.Split(" ");
            if(ruleParts[0].Equals("rewrite", StringComparison.CurrentCultureIgnoreCase)) {
                _ruleType = 1;
                _pattern = new Regex(ruleParts[1]);
                _rewriteData = ruleParts[2];
                if (ruleParts.Length == 4) _flag = ruleParts[3];
            } else if(ruleParts[0].Equals("return", StringComparison.CurrentCultureIgnoreCase)) {
                _ruleType = 2;
                _statusCode = int.Parse(ruleParts[1]);
                _rewriteData = ruleParts[2];
                _flag = string.Empty;
            } 
        }

        public byte MatchRule(string request, HtcHttpContext context, out string newRequest) {
            if (_ruleType == 1) {
                var requestParts = request.Split("/");
                if (_pattern.IsMatch(request)) {
                    byte response = 0;
                    var rewriteQuery = _rewriteData.Split("?");
                    request = rewriteQuery[0].Replace("$scheme", context.Request.Scheme);
                    var queryParts = rewriteQuery[1].Split("&");
                    if (rewriteQuery.Length == 2) {
                        foreach (var query in queryParts) {
                            var queryData = query.Split("=");
                            var key = queryData[0];
                            var value = queryData[1];
                            for (var p = 1; p < requestParts.Length; p++) {
                                value = value.Replace($"${p}", requestParts[p]);
                            }
                            context.Request.Query.Add(key, value);
                        }
                    }
                    if (_flag.Equals("last", StringComparison.CurrentCultureIgnoreCase)) response = 0;
                    if (_flag.Equals("break", StringComparison.CurrentCultureIgnoreCase)) response = 1;
                    if (_flag.Equals("redirect", StringComparison.CurrentCultureIgnoreCase)) response = 2;
                    if (_flag.Equals("permanent", StringComparison.CurrentCultureIgnoreCase)) response = 3;
                    newRequest = request;
                    return response;
                }
            } else if (_ruleType == 2) {
                newRequest = request;
                return 0;
            }
            newRequest = request;
            return 0;
        }
    }
}
