using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using HtcSharp.Core.Components.Http;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Interfaces.Http;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Utils;
using HtcSharp.Core.Utils.Http;

namespace HtcSharp.Core.Models.Http.Directives {
    public class TryFilesDirective : IDirective {

        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<string> _files;
        private readonly HttpLocationManager _httpLocationManager;

        public TryFilesDirective(IReadOnlyList<string> files, HttpLocationManager httpLocationManager) {
            _httpLocationManager = httpLocationManager;
            _files = new List<string>();
            for (var i = 1; i < files.Count; i++) {
                _files.Add(files[i]);
            }
        }

        public void Execute(HtcHttpContext context) {
            foreach (var file in _files) {
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
                context.Request.TranslatedPath = Path.GetFullPath(Path.Combine(context.ServerInfo.RootPath, tempPath.Remove(0, 1)));
                if (File.Exists(context.Request.TranslatedPath)) {
                    var extension = Path.GetExtension(context.Request.TranslatedPath);
                    if (UrlMapper.ExtensionPlugins.ContainsKey(extension.ToLower())) {
                        if (UrlMapper.ExtensionPlugins[extension.ToLower()].OnHttpExtensionRequest(context, context.Request.TranslatedPath, extension.ToLower())) {
                            context.ErrorMessageManager.SendError(context, 500);
                        }
                    } else {
                        try {
                            HttpIoUtils.CallFile(context, context.Request.TranslatedPath);
                        } catch {
                            context.ErrorMessageManager.SendError(context, 500);
                        }
                    }
                } else if (Directory.Exists(context.Request.TranslatedPath)) {
                    if (!context.Request.RequestPath.EndsWith('/')) {
                        context.Response.Redirect(context.Request.RequestPath + "/");
                    }
                    //context.ErrorMessageManager.SendError(context, 404);
                    // Do indexer
                }
            }
        }
    }
}
