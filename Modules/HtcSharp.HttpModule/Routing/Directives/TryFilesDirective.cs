using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.IO;
using HtcSharp.HttpModule.Routing.Abstractions;
using HtcSharp.HttpModule.Routing.Error;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class TryFilesDirective : IDirective {
        private readonly List<string> _files;
        private readonly HttpLocationManager _httpLocationManager;

        public TryFilesDirective(IReadOnlyList<string> files, HttpLocationManager httpLocationManager) {
            _httpLocationManager = httpLocationManager;
            _files = new List<string>();
            for (var i = 1; i < files.Count; i++) {
                _files.Add(files[i]);
            }
        }

        public async Task Execute(HttpContext context) {
            foreach (string file in _files) {
                string tempPath = HttpIO.ReplaceVars(context, file);
                if (tempPath[0].Equals('=')) {
                    if (int.TryParse(tempPath.Remove(0, 1), out int statusCode)) {
                        await context.ServerInfo.ErrorMessageManager.SendError(context, statusCode);
                        return;
                    }
                    await context.ServerInfo.ErrorMessageManager.SendError(context, 500);
                    return;
                }
                if (tempPath[0].Equals('@')) {
                    foreach (var location in _httpLocationManager.Locations) {
                        if (!location.Key.Equals(tempPath, StringComparison.CurrentCultureIgnoreCase)) continue;
                        await location.Execute(context);
                        return;
                    }
                }
                context.Request.TranslatedPath = Path.GetFullPath(Path.Combine(context.ServerInfo.RootPath, tempPath.Remove(0, 1)));
                if (File.Exists(context.Request.TranslatedPath)) {
                    string extension = Path.GetExtension(context.Request.TranslatedPath);
                    if (UrlMapper.ExtensionPlugins.ContainsKey(extension.ToLower())) {
                        await UrlMapper.ExtensionPlugins[extension.ToLower()].OnHttpExtensionRequest(context, context.Request.TranslatedPath, extension.ToLower());
                    } else {
                        try {
                            await HttpIO.SendFile(context, context.Request.TranslatedPath);
                        } catch {
                            await context.ServerInfo.ErrorMessageManager.SendError(context, 500);
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
