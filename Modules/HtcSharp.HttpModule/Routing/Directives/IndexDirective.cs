using System.Collections.Generic;
using System.IO;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.IO;
using HtcSharp.HttpModule.Routing.Abstractions;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class IndexDirective : IDirective {

        private readonly List<string> _indexes;

        public IndexDirective(IReadOnlyList<string> index) {
            _indexes = new List<string>();
            for (var i = 1; i < index.Count; i++) {
                _indexes.Add(index[i]);
            }
        }

        public void Execute(HttpContext context) {
            foreach (var i in _indexes) {
                var index = HttpIO.ReplaceVars(context, i);
                if (index.Equals("$internal_indexes")) {
                    foreach (var j in UrlMapper.IndexFiles) {
                        var indexPath = Path.Combine(context.ServerInfo.RootPath, j[0].Equals('/') ? j.Remove(0, 1) : Path.Combine(context.Request.Path, j).Remove(0, 1));
                        if (!File.Exists(indexPath)) continue;
                        var extension = Path.GetExtension(indexPath);
                        context.Request.RequestFilePath = indexPath;
                        if (UrlMapper.ExtensionPlugins.TryGetValue(extension.ToLower(), out var plugin)) {
                            if (!plugin.OnHttpExtensionRequest(context, indexPath, extension.ToLower())) continue;
                            context.ErrorMessageManager.SendError(context, 500);
                            return;
                        }
                        try {
                            HttpIO.CallFile(context, indexPath);
                        } catch {
                            context.ErrorMessageManager.SendError(context, 500);
                        }
                        return;
                    }
                } else {
                    var indexPath = Path.Combine(context.ServerInfo.RootPath, index[0].Equals('/') ? index.Remove(0, 1) : Path.Combine(context.Request.Path, index).Remove(0, 1));
                    if (!File.Exists(indexPath)) continue;
                    var extension = Path.GetExtension(indexPath);
                    context.Request.RequestFilePath = indexPath;
                    if (UrlMapper.ExtensionPlugins.TryGetValue(extension.ToLower(), out var plugin)) {
                        if (!plugin.OnHttpExtensionRequest(context, indexPath, extension.ToLower())) continue;
                        context.ErrorMessageManager.SendError(context, 500);
                        return;
                    }
                    try {
                        HttpIO.CallFile(context, indexPath);
                    } catch {
                        context.ErrorMessageManager.SendError(context, 500);
                    }
                    return;
                }
            }
        }
    }
}
