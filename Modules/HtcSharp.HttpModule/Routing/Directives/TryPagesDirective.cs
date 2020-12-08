using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing.Abstractions;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class TryPagesDirective : IDirective {
        // SourceTools-Start
        // Ignore-Copyright
        // SourceTools-End
        private readonly List<string> _pages;
        private readonly HttpLocationManager _httpLocationManager;

        public TryPagesDirective(string pages, HttpLocationManager httpLocationManager) {
            _httpLocationManager = httpLocationManager;
            _pages = new List<string>();
            string[] pagesData = pages.Split(" ");
            foreach (string page in pagesData) {
                _pages.Add(page);
            }
        }

        public async Task Execute(HttpContext context) {
            foreach (string tempPath in _pages.Select(file => file.ReplaceHttpContextVars(context))) {
                switch (tempPath[0]) {
                    case '=' when int.TryParse(tempPath.Remove(0, 1), out int statusCode):
                        await context.ServerInfo.ErrorMessageManager.SendError(context, statusCode);
                        context.Response.HasFinished = true;
                        return;
                    case '=':
                        await context.ServerInfo.ErrorMessageManager.SendError(context, 500);
                        context.Response.HasFinished = true;
                        return;
                    case '@': {
                        foreach (var location in _httpLocationManager.Locations.Where(location => location.Key.Equals(tempPath, StringComparison.CurrentCultureIgnoreCase))) {
                            await location.Execute(context);
                            context.Response.HasFinished = true;
                            return;
                        }

                        break;
                    }
                }

                if (!UrlMapper.RegisteredPages.ContainsKey(tempPath)) continue;
                await UrlMapper.RegisteredPages[tempPath].OnHttpPageRequest(context, tempPath);
                context.Response.HasFinished = true;
            }
        }
    }
}