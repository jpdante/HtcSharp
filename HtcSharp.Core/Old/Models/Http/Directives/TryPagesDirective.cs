using System;
using System.Collections.Generic;
using System.Reflection;
using HtcSharp.Core.Old.Components.Http;
using HtcSharp.Core.Old.Helpers.Http;
using HtcSharp.Core.Old.Interfaces.Http;
using HtcSharp.Core.Old.Logging;
using HtcSharp.Core.Old.Utils.Http;

namespace HtcSharp.Core.Old.Models.Http.Directives {
    public class TryPagesDirective : IDirective {

        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<string> _pages;
        private readonly HttpLocationManager _httpLocationManager;

        public TryPagesDirective(IReadOnlyList<string> pages, HttpLocationManager httpLocationManager) {
            _httpLocationManager = httpLocationManager;
            _pages = new List<string>();
            for (var i = 1; i < pages.Count; i++) {
                _pages.Add(pages[i]);
            }
        }

        public void Execute(HtcHttpContext context) {
            foreach (var file in _pages) {
                var tempPath = HttpIoUtils.ReplaceVars(context, file);
                if (tempPath[0].Equals('=')) {
                    if (int.TryParse(tempPath.Remove(0, 1), out var statusCode)) {
                        context.ErrorMessageManager.SendError(context, statusCode);
                        return;
                    }
                    context.ErrorMessageManager.SendError(context, 500);
                    return;
                }
                if (tempPath[0].Equals('@')) {
                    foreach (var location in _httpLocationManager.Locations) {
                        if (!location.Key.Equals(tempPath, StringComparison.CurrentCultureIgnoreCase)) continue;
                        location.Execute(context);
                        return;
                    }
                }
                if (!UrlMapper.RegisteredPages.ContainsKey(tempPath.ToLower())) continue;
                if (UrlMapper.RegisteredPages[tempPath.ToLower()].OnHttpPageRequest(context, tempPath.ToLower())) {
                    context.ErrorMessageManager.SendError(context, 500);
                }
            }
        }
    }
}
