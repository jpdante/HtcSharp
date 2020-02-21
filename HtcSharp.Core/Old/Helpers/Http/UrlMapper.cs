using System.Collections.Generic;
using System.Reflection;
using HtcSharp.Core.Old.Interfaces.Plugin;
using HtcSharp.Core.Old.Logging;

namespace HtcSharp.Core.Old.Helpers.Http {
    public static class UrlMapper {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        internal static readonly List<string> IndexFiles = new List<string>();
        internal static readonly Dictionary<string, IHttpEvents> ExtensionPlugins = new Dictionary<string, IHttpEvents>();
        internal static readonly Dictionary<string, IHttpEvents> RegisteredPages = new Dictionary<string, IHttpEvents>();

        public static void RegisterIndexFile(string file) => IndexFiles.Add(file.ToLower());

        public static void UnRegisterIndexFile(string file) => IndexFiles.Remove(file.ToLower());

        public static void RegisterPluginExtension(string extension, IHttpEvents plugin) => ExtensionPlugins.Add(extension.ToLower(), plugin);

        public static void UnRegisterPluginExtension(string extension) => ExtensionPlugins.Remove(extension.ToLower());

        public static void RegisterPluginPage(string page, IHttpEvents plugin) => RegisteredPages.Add(page, plugin);

        public static void UnRegisterPluginPage(string page) => RegisteredPages.Remove(page);

        /*public static void ProcessRequest(HtcHttpContext httpContext, HttpServerInfo serverInfo) {
            var root = serverInfo.Root;
            httpContext.Request.RequestPath = httpContext.Request.Path;
            httpContext.Request.RequestFilePath = httpContext.Request.RequestPath;
            if (RegisteredPages.ContainsKey(httpContext.Request.RequestPath.ToLower())) {
                if (RegisteredPages[httpContext.Request.RequestPath.ToLower()].OnHttpPageRequest(httpContext, httpContext.Request.RequestPath.ToLower())) {
                    httpContext.ErrorMessageManager.SendError(httpContext, 500);
                }
            } else {
                httpContext.Request.TranslatedPath = Path.GetFullPath(Path.Combine(root, httpContext.Request.RequestPath.Remove(0, 1)));
                if (File.Exists(httpContext.Request.TranslatedPath)) {
                    var extension = Path.GetExtension(httpContext.Request.TranslatedPath);
                    if (ExtensionPlugins.ContainsKey(extension.ToLower())) {
                        if (ExtensionPlugins[extension.ToLower()].OnHttpExtensionRequest(httpContext, httpContext.Request.TranslatedPath, extension.ToLower())) {
                            httpContext.ErrorMessageManager.SendError(httpContext, 500);
                        }
                    } else {
                        try {
                            CallFile(httpContext, httpContext.Request.TranslatedPath);
                        } catch (Exception ex) {
                            Logger.Error(ex);
                            httpContext.ErrorMessageManager.SendError(httpContext, 500);
                        }
                    }
                } else {
                    if (Directory.Exists(httpContext.Request.TranslatedPath)) {
                        if (httpContext.Request.Path.Length > 0 && httpContext.Request.Path[httpContext.Request.Path.Length - 1] != '/') httpContext.Response.Redirect($"{httpContext.Request.Path}/");
                        else CallIndex(httpContext, false);
                    } else {
                        httpContext.ErrorMessageManager.SendError(httpContext, 404);
                    }
                }
            }
        }

        public static void CallIndex(HtcHttpContext httpContext, bool allowDirectoryIndexer) {
            foreach (var indexFile in IndexFiles) {
                var indexRequestPath = Path.Combine(httpContext.Request.Path, indexFile);
                var indexFilePath = Path.Combine(httpContext.ServerInfo.RootPath, indexRequestPath.Remove(0, 1));
                var extension = Path.GetExtension(indexFilePath);
                if (RegisteredPages.ContainsKey(indexRequestPath.ToLower())) {
                    httpContext.Request.TranslatedPath = indexFilePath;
                    if (RegisteredPages[indexRequestPath.ToLower()].OnHttpPageRequest(httpContext, indexRequestPath.ToLower())) {
                        httpContext.ErrorMessageManager.SendError(httpContext, 500);
                    }
                    return;
                } else {
                    httpContext.Request.RequestFilePath = indexRequestPath;
                    if (!File.Exists(indexFilePath)) continue;
                    httpContext.Request.TranslatedPath = indexFilePath;
                    if (ExtensionPlugins.TryGetValue(extension.ToLower(), out var plugin)) {
                        if (plugin.OnHttpExtensionRequest(httpContext, indexFilePath, extension.ToLower())) {
                            httpContext.ErrorMessageManager.SendError(httpContext, 500);
                            return;
                        }
                    } else {
                        try {
                            CallFile(httpContext, indexFilePath);
                        } catch (Exception ex) {
                            Logger.Error(ex);
                            httpContext.ErrorMessageManager.SendError(httpContext, 500);
                        }
                        return;
                    }
                }
            }
            httpContext.ErrorMessageManager.SendError(httpContext, allowDirectoryIndexer ? 500 : 403);
        }*/
    }
}
