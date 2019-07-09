using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Interfaces.Http;
using HtcSharp.Core.Utils;

namespace HtcSharp.Core.Models.Http.Directives {
    public class IndexDirective : IDirective {

        private readonly List<string> _indexes;

        public IndexDirective(IReadOnlyList<string> index) {
            _indexes = new List<string>();
            for (var i = 1; i < index.Count; i++) {
                _indexes.Add(index[i]);
            }
        }

        public void Execute(HtcHttpContext context) {
            foreach (var i in _indexes) {
                if (i.Equals("$internal_indexes")) {
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
                            HttpIoUtils.CallFile(context, indexPath);
                        } catch {
                            context.ErrorMessageManager.SendError(context, 500);
                        }
                        return;
                    }
                } else {
                    var indexPath = Path.Combine(context.ServerInfo.RootPath, i[0].Equals('/') ? i.Remove(0, 1) : Path.Combine(context.Request.Path, i).Remove(0, 1));
                    if (!File.Exists(indexPath)) continue;
                    var extension = Path.GetExtension(indexPath);
                    context.Request.RequestFilePath = indexPath;
                    if (UrlMapper.ExtensionPlugins.TryGetValue(extension.ToLower(), out var plugin)) {
                        if (!plugin.OnHttpExtensionRequest(context, indexPath, extension.ToLower())) continue;
                        context.ErrorMessageManager.SendError(context, 500);
                        return;
                    }
                    try {
                        HttpIoUtils.CallFile(context, indexPath);
                    } catch {
                        context.ErrorMessageManager.SendError(context, 500);
                    }
                    return;
                }
            }
        }
    }
}
