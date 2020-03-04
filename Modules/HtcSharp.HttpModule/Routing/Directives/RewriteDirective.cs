using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.IO;
using HtcSharp.HttpModule.Routing.Abstractions;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class ReWriteDirective : IDirective {

        private readonly Regex _pattern;
        private readonly string _rewriteData;
        private readonly string _flag;

        public ReWriteDirective(IReadOnlyList<string> rewrite) {
            _pattern = new Regex(rewrite[1]);
            _rewriteData = rewrite[2];
            if (rewrite.Count == 4) _flag = rewrite[3];
        }

        public Task Execute(HttpContext context) {
            var match = _pattern.Match(context.Request.Path);
            if (!match.Success) return Task.CompletedTask;
            var newRequest = HttpIO.ReplaceVars(context, _rewriteData);
            for (var i = 0; i < match.Captures.Count; i++) {
                newRequest = newRequest.Replace($"${i + 1}", match.Captures[i].Value);
            }
            /*foreach (Match match in _pattern.Matches(context.Request.Path)) {
                newRequest = newRequest.Replace($"${match.Name}", match.Value);
            }*/
            /*var rewriteQuery = _rewriteData.Split("?");
                request = rewriteQuery[0].Replace("$scheme", context.Request.Scheme);
                if (rewriteQuery.Length == 1) {
                    request = _rewriteData;
                    Logger.Info($"{request}");
                    for (var p = 1; p < requestParts.Length; p++) {
                        request = request.Replace($"${p}", requestParts[p]);
                    }
                    Logger.Info($"{request}");
                } else if (rewriteQuery.Length == 2) {
                    var queryParts = rewriteQuery[1].Split("&");
                    foreach (var query in queryParts) {
                        var queryData = query.Split("=");
                        var key = queryData[0];
                        var value = queryData[1];
                        for (var p = 1; p < requestParts.Length; p++) {
                            value = value.Replace($"${p}", requestParts[p]);
                        }
                        context.Request.Query.Add(key, value);
                    }
                }*/
            /*if (_flag.Equals("last", StringComparison.CurrentCultureIgnoreCase)) {
                context.Request
            }
            if (_flag.Equals("break", StringComparison.CurrentCultureIgnoreCase)) {

            }*/
            if (_flag.Equals("redirect", StringComparison.CurrentCultureIgnoreCase)) {
                context.Response.StatusCode = 302;
                context.Response.Headers.Add("Location", newRequest);
            }
            if (_flag.Equals("permanent", StringComparison.CurrentCultureIgnoreCase)) {
                context.Response.StatusCode = 301;
                context.Response.Headers.Add("Location", newRequest);
            }
            context.Request.RequestPath = newRequest;
            return Task.CompletedTask;
        }
    }
}
