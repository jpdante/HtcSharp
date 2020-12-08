using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing.Abstractions;

namespace HtcSharp.HttpModule.Routing.Directives {

    public enum ReWriteFlag {
        None,
        Redirect,
        Permanent
    }

    public class ReWriteDirective : IDirective {
        // SourceTools-Start
        // Ignore-Copyright
        // SourceTools-End
        private readonly Regex _pattern;
        private readonly string _rewriteData;
        private readonly ReWriteFlag _flag;

        public ReWriteDirective(string rewrite) {
            string[] rewriteData = rewrite.Split(" ", 2);
            if (rewriteData.Length < 2) return;
            _pattern = new Regex(rewriteData[0]);
            _rewriteData = rewriteData[1];
            if (rewriteData.Length != 3) return;
            if (rewriteData[2].Equals("redirect", StringComparison.CurrentCultureIgnoreCase)) _flag = ReWriteFlag.Redirect;
            else if (rewriteData[2].Equals("permanent", StringComparison.CurrentCultureIgnoreCase)) _flag = ReWriteFlag.Permanent;
        }

        public Task Execute(HttpContext context) {
            var match = _pattern.Match(context.Request.Path);
            if (!match.Success) return Task.CompletedTask;
            string newRequest = _rewriteData.ReplaceHttpContextVars(context);
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
            switch (_flag) {
                case ReWriteFlag.Redirect:
                    context.Response.StatusCode = 302;
                    context.Response.Headers.Add("Location", newRequest);
                    context.Response.HasFinished = true;
                    break;
                case ReWriteFlag.Permanent:
                    context.Response.StatusCode = 301;
                    context.Response.Headers.Add("Location", newRequest);
                    context.Response.HasFinished = true;
                    break;
            }

            context.Request.RequestPath = newRequest;
            return Task.CompletedTask;
        }
    }
}