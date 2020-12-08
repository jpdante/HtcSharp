using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing.Abstractions;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class TryFilesDirective : IDirective {
        // SourceTools-Start
        // Ignore-Copyright
        // SourceTools-End
        private readonly StaticFileFactory _staticFileFactory;
        private readonly List<string> _files;
        private readonly HttpLocationManager _httpLocationManager;

        public TryFilesDirective(StaticFileFactory staticFileFactory, string files, HttpLocationManager httpLocationManager) {
            _staticFileFactory = staticFileFactory;
            _httpLocationManager = httpLocationManager;
            _files = new List<string>();
            string[] filesData = files.Split(" ");
            foreach (string file in filesData) {
                _files.Add(file);
            }
        }

        public async Task Execute(HttpContext context) {
            foreach (string tempPath in _files.Select(file => file.ReplaceHttpContextVars(context))) {
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

                context.Request.TranslatedPath = Path.GetFullPath(Path.Combine(context.ServerInfo.RootPath, tempPath.Remove(0, 1)));
                if (File.Exists(context.Request.TranslatedPath)) {
                    string extension = Path.GetExtension(context.Request.TranslatedPath);
                    if (UrlMapper.ExtensionPlugins.ContainsKey(extension.ToLower())) {
                        await UrlMapper.ExtensionPlugins[extension.ToLower()].OnHttpExtensionRequest(context, context.Request.TranslatedPath, extension.ToLower());
                        context.Response.HasFinished = true;
                    } else {
                        try {
                            await _staticFileFactory.ServeStaticFile(context, context.Request.TranslatedPath);
                        } catch {
                            //await context.ServerInfo.ErrorMessageManager.SendError(context, 500);
                        }
                        context.Response.HasFinished = true;
                    }
                } else if (Directory.Exists(context.Request.TranslatedPath)) {
                    if (!context.Request.RequestPath.EndsWith('/')) {
                        context.Response.Redirect(context.Request.RequestPath + "/");
                        context.Response.HasFinished = true;
                    }
                    //context.ErrorMessageManager.SendError(context, 404);
                    // Do indexer
                }
            }
        }
    }
}