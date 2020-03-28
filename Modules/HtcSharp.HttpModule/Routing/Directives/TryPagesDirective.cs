using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.IO;
using HtcSharp.HttpModule.Routing.Abstractions;
using HtcSharp.HttpModule.Routing.Error;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class TryPagesDirective : IDirective {

        private readonly List<string> _pages;
        private readonly HttpLocationManager _httpLocationManager;

        public TryPagesDirective(IReadOnlyList<string> pages, HttpLocationManager httpLocationManager) {
            _httpLocationManager = httpLocationManager;
            _pages = new List<string>();
            for (var i = 1; i < pages.Count; i++) {
                _pages.Add(pages[i]);
            }
        }

        public async Task Execute(HttpContext context) {
            foreach (string file in _pages) {
                string tempPath = HttpIO.ReplaceVars(context, file);
                if (tempPath[0].Equals('=')) {
                    if (int.TryParse(tempPath.Remove(0, 1), out int statusCode)) {
                        await context.ServerInfo.ErrorMessageManager.SendError(context, statusCode);
                        context.Response.HasFinished = true;
                        return;
                    }
                    await context.ServerInfo.ErrorMessageManager.SendError(context, 500);
                    context.Response.HasFinished = true;
                    return;
                }
                if (tempPath[0].Equals('@')) {
                    foreach (var location in _httpLocationManager.Locations) {
                        if (!location.Key.Equals(tempPath, StringComparison.CurrentCultureIgnoreCase)) continue;
                        await location.Execute(context);
                        context.Response.HasFinished = true;
                        return;
                    }
                }
                if (!UrlMapper.RegisteredPages.ContainsKey(tempPath.ToLower())) continue;
                await UrlMapper.RegisteredPages[tempPath.ToLower()].OnHttpPageRequest(context, tempPath.ToLower());
                context.Response.HasFinished = true;
            }
        }
    }
}
