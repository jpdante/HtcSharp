using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HtcSharp.Core.Components.Http;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Interfaces.Http;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Utils;
using HtcSharp.Core.Utils.Http;

namespace HtcSharp.Core.Models.Http.Directives {
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
                if (file[0].Equals('=')) {
                    if (int.TryParse(file.Remove(0, 1), out var statusCode)) {
                        context.ErrorMessageManager.SendError(context, statusCode);
                        return;
                    }
                    context.ErrorMessageManager.SendError(context, 500);
                    return;
                }
                if (file[0].Equals('@')) {
                    foreach (var location in _httpLocationManager.Locations) {
                        if (!location.Key.Equals(file, StringComparison.CurrentCultureIgnoreCase)) continue;
                        location.Execute(context);
                        return;
                    }
                }
                if (!UrlMapper.RegisteredPages.ContainsKey(context.Request.RequestPath.ToLower())) continue;
                if (UrlMapper.RegisteredPages[context.Request.RequestPath.ToLower()].OnHttpPageRequest(context, context.Request.RequestPath.ToLower())) {
                    context.ErrorMessageManager.SendError(context, 500);
                }
            }
        }
    }
}
