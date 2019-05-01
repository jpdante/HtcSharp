using HtcSharp.Core.Interfaces.Plugin;
using HtcSharp.Core.IO;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Models.Http;
using HtcSharp.Core.Models.Http.Pages;
using HtcSharp.Core.Models.ReWriter;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HtcSharp.Core.Helpers.Http {
    public static class UrlMapper {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly List<string> IndexFiles = new List<string>();
        private static readonly Dictionary<string, IHttpEvents> ExtensionPlugins = new Dictionary<string, IHttpEvents>();
        private static readonly Dictionary<string, IHttpEvents> RegisteredPages = new Dictionary<string, IHttpEvents>();
//         private static Dictionary<string, FolderConfig> FolderConfigs = new Dictionary<string, FolderConfig>();
//         private static FolderConfig DefaultConfig = new FolderConfig();

        public static void RegisterIndexFile(string file) {
            IndexFiles.Add(file.ToLower());
        }

        public static void UnRegisterIndexFile(string file) {
            IndexFiles.Remove(file.ToLower());
        }

        public static void RegisterPluginExtension(string extension, IHttpEvents plugin) {
            ExtensionPlugins.Add(extension.ToLower(), plugin);
        }

        public static void UnRegisterPluginExtension(string extension) {
            ExtensionPlugins.Remove(extension.ToLower());
        }

        public static void RegisterPluginPage(string page, IHttpEvents plugin) {
            RegisteredPages.Add(page, plugin);
        }

        public static void UnRegisterPluginPage(string page) {
            RegisteredPages.Remove(page);
        }
// 
//         public static void DoFolderConfig(HTCHttpContext context, HttpServerInfo serverInfo, out string definitiveUrl, out FolderConfig pathConfig) {
//             string tempRoot = serverInfo.Root;
//             string tempUrlPath = context.Request.Path.ToString();
//             string tempPath = Path.GetDirectoryName(Path.GetFullPath(Path.Combine(tempRoot, tempUrlPath.Remove(0, 1))));
//             if(FolderConfigs.ContainsKey(tempUrlPath)) {
//                 FolderConfig config = FolderConfigs[tempUrlPath];
//                 config.CheckConfigChange();
//                 definitiveUrl = config.Rewrite(tempUrlPath, context);
//                 pathConfig = config;
//                 return;
//             } else {
//                 if (File.Exists(Path.Combine(tempPath, "htcconf.json"))) {
//                     FolderConfig config = new FolderConfig(tempUrlPath, Path.Combine(tempPath, "htcconf.json"));
//                     FolderConfigs.Add(tempUrlPath, config);
//                     config.CheckConfigChange();
//                     definitiveUrl = config.Rewrite(tempUrlPath, context);
//                     pathConfig = config;
//                     return;
//                 } else {
//                     definitiveUrl = tempUrlPath;
//                     pathConfig = DefaultConfig;
//                     return;
//                 }
//             }
//         }

        public static void ProcessRequest(HtcHttpContext httpContext, HttpServerInfo serverInfo) {
            string root = serverInfo.Root;
            string requestedUrl = null;
            if (serverInfo.GetHttpReWriter != null) {
                if (serverInfo.GetHttpReWriter.Rewrite(httpContext, out requestedUrl)) {
                    Close(httpContext);
                    return;
                }
            } else {
                requestedUrl = httpContext.Request.Path.ToString();
            }
            if (RegisteredPages.ContainsKey(requestedUrl.ToLower())) {
                if (RegisteredPages[requestedUrl.ToLower()].OnHttpPageRequest(httpContext, requestedUrl.ToLower())) {
                    Default5xx.Call500(httpContext);
                }
            } else {
                string requestPath = Path.GetFullPath(Path.Combine(root, requestedUrl.Remove(0, 1)));
                if (File.Exists(requestPath)) {
                    string extension = Path.GetExtension(requestPath);
                    if (ExtensionPlugins.ContainsKey(extension.ToLower())) {
                        if (ExtensionPlugins[extension.ToLower()].OnHttpExtensionRequest(httpContext, requestPath, extension.ToLower())) {
                            Default5xx.Call500(httpContext);
                        }
                    } else {
                        try {
                            CallFile(httpContext, requestPath);
                        } catch (Exception ex) {
                            Logger.Error(ex);
                            Default5xx.Call500(httpContext);
                        }
                    }
                } else {
                    if (Directory.Exists(requestPath)) {
                        CallIndex(httpContext, requestPath, requestedUrl, false);
                    } else {
                        Default4xx.Call404(httpContext);
                    }
                }
            }
            Close(httpContext);
        }

        public static void Close(HtcHttpContext httpContext) {
            try {
                httpContext.Response.OutputStream.Close();
            } catch {
                // ignored
            }
        }

        public static void CallIndex(HtcHttpContext httpContext, string rootPath, string requestPath, bool allowDirectoryIndexer) {
            foreach (var indexFile in IndexFiles) {
                var indexRequestPath = Path.Combine(requestPath, indexFile);
                var indexFilePath = Path.Combine(rootPath, indexRequestPath.Remove(0, 1));
                string extension = Path.GetExtension(indexFilePath);
                if (RegisteredPages.ContainsKey(indexRequestPath.ToLower())) {
                    if (RegisteredPages[indexRequestPath.ToLower()].OnHttpPageRequest(httpContext, indexRequestPath.ToLower())) {
                        Default5xx.Call500(httpContext);
                    }
                    return;
                } else {
                    if (!File.Exists(indexFilePath)) continue;
                    if (ExtensionPlugins.TryGetValue(extension.ToLower(), out var plugin)) {
                        if (plugin.OnHttpExtensionRequest(httpContext, indexFilePath, extension.ToLower())) {
                            Default5xx.Call500(httpContext);
                        }
                    } else {
                        try {
                            CallFile(httpContext, indexFilePath);
                        } catch (Exception ex) {
                            Logger.Error(ex);
                            Default5xx.Call500(httpContext);
                        }
                    }
                }
            }
            if (allowDirectoryIndexer) {
                Default5xx.Call500(httpContext);
            } else {
                Default4xx.Call403(httpContext);
            }
        }

        public static void CallFile(HtcHttpContext httpContext, string requestPath) {
            using (FileBuffer fileBuffer = new FileBuffer(requestPath, 2048)) {
                 ContentType contentType = ContentType.DEFAULT.FromExtension(requestPath);
                // 7 * 24 Hour * 60 Min * 60 Sec = 604800 Sec;
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                httpContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                //context.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(requestPath).ToString("r"));
                httpContext.Response.Headers.Add("Server", "HtcSharp");
                //context.Response.Headers.Add("Cache-Control", "max-age=604800");
                httpContext.Response.ContentType = contentType.ToValue();
                httpContext.Response.StatusCode = 200;
                Tuple<long, long> Range = GetRange(httpContext, fileBuffer);
                if (UseGzip(httpContext, fileBuffer.Lenght, contentType)) {
                    httpContext.Response.Headers.Add("Content-Encoding", "gzip");
                    using (GZipStream gzipStream = new GZipStream(httpContext.Response.OutputStream, CompressionMode.Compress, false)) {
                        fileBuffer.CopyToStream(gzipStream, Range.Item1, Range.Item2);
                    }
                } else {
                    httpContext.Response.ContentLength = Range.Item2 - Range.Item1;
                    fileBuffer.CopyToStream(httpContext.Response.OutputStream, Range.Item1, Range.Item2);
                }
            }
        }

        private static bool UseGzip(HtcHttpContext httpContext, long fileSize, ContentType contentType) {
            var header = GetHeader(httpContext, "Accept-Encoding");
            return fileSize > (1024 * 5) && fileSize < (1024 * 1024 * 5) && header != null && header.Contains("gzip", StringComparison.CurrentCultureIgnoreCase);
        }

        private static Tuple<long, long> GetRange(HtcHttpContext httpContext, FileBuffer fileBuffer) {
            var rangeData = GetHeader(httpContext, "Range");
            long startRange = -1;
            long endRange = -1;
            if (rangeData != null) {
                var rangeHeader = rangeData.Replace("bytes=", "");
                var range = rangeHeader.Split('-');
                startRange = long.Parse(range[0]);
                if (range[1].Trim().Length > 0) long.TryParse(range[1], out endRange);
                if (endRange == -1) endRange = fileBuffer.Lenght;
            } else {
                startRange = 0;
                endRange = fileBuffer.Lenght;
            }
            return new Tuple<long, long>(startRange, endRange);
        }

        private static string GetHeader(HtcHttpContext httpContext, string name) {
            foreach (var key in httpContext.Request.Headers.Keys) {
                if (key.Equals(name, StringComparison.CurrentCultureIgnoreCase)) {
                    return httpContext.Request.Headers[key];
                }
            }
            return null;
        }
    }
}
