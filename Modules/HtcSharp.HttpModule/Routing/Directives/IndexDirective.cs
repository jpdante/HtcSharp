using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.IO;
using HtcSharp.HttpModule.Routing.Abstractions;
using HtcSharp.HttpModule.Routing.Error;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class IndexDirective : IDirective {

        private readonly List<string> _indexes;

        public IndexDirective(IReadOnlyList<string> index) {
            _indexes = new List<string>();
            for (var i = 1; i < index.Count; i++) {
                _indexes.Add(index[i]);
            }
        }

        public async Task Execute(HttpContext context) {
            foreach (string i in _indexes) {
                string index = HttpIO.ReplaceVars(context, i);
                if (index.Equals("$internal_indexes")) {
                    foreach (string j in UrlMapper.IndexFiles) {
                        string indexPath = Path.Combine(context.ServerInfo.RootPath, j[0].Equals('/') ? j.Remove(0, 1) : Path.Combine(context.Request.Path, j).Remove(0, 1));
                        if (!File.Exists(indexPath)) continue;
                        string extension = Path.GetExtension(indexPath);
                        context.Request.RequestFilePath = indexPath;
                        if (UrlMapper.ExtensionPlugins.TryGetValue(extension.ToLower(), out var plugin)) {
                            await plugin.OnHttpExtensionRequest(context, indexPath, extension.ToLower());
                            context.Response.HasFinished = true;
                            return;
                        }
                        try {
                            await HttpIO.SendFile(context, indexPath);
                            context.Response.HasFinished = true;
                        } catch(Exception) {
                            await context.ServerInfo.ErrorMessageManager.SendError(context, 500);
                            context.Response.HasFinished = true;
                        }
                        return;
                    }
                } else {
                    string indexPath = Path.Combine(context.ServerInfo.RootPath, index[0].Equals('/') ? index.Remove(0, 1) : Path.Combine(context.Request.Path, index).Remove(0, 1));
                    if (!File.Exists(indexPath)) continue;
                    string extension = Path.GetExtension(indexPath);
                    context.Request.RequestFilePath = indexPath;
                    if (UrlMapper.ExtensionPlugins.TryGetValue(extension.ToLower(), out var plugin)) {
                        await plugin.OnHttpExtensionRequest(context, indexPath, extension.ToLower());
                        context.Response.HasFinished = true;
                        return;
                    }
                    try {
                        await HttpIO.SendFile(context, indexPath);
                        context.Response.HasFinished = true;
                    } catch {
                        await context.ServerInfo.ErrorMessageManager.SendError(context, 500);
                        context.Response.HasFinished = true;
                    }
                    return;
                }
            }
        }
    }
}
