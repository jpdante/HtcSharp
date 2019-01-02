using HTCSharp.Core.Interfaces.Plugin;
using HTCSharp.Core.IO;
using HTCSharp.Core.Logging;
using HTCSharp.Core.Models.Http;
using HTCSharp.Core.Models.Http.Pages;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HTCSharp.Core.Helpers.Http {
    public static class URLMapping {

        private static readonly ILog _Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        private static List<string> IndexFiles = new List<string>();
        private static Dictionary<string, IHttpEvents> ExtensionPlugins = new Dictionary<string, IHttpEvents>();
        private static Dictionary<string, IHttpEvents> RegisteredPages = new Dictionary<string, IHttpEvents>();

        public static void RegisterIndexFile(string file) {
            IndexFiles.Add(file.ToLower());
        }

        public static void UnregisterIndexFile(string file) {
            IndexFiles.Remove(file.ToLower());
        }

        public static void RegisterPluginExtension(string extension, IHttpEvents plugin) {
            ExtensionPlugins.Add(extension.ToLower(), plugin);
        }

        public static void UnregisterPluginExtension(string extension) {
            ExtensionPlugins.Remove(extension.ToLower());
        }

        public static void RegisterPluginPage(string page, IHttpEvents plugin) {
            RegisteredPages.Add(page, plugin);
        }

        public static void UnregisterPluginPage(string page) {
            RegisteredPages.Remove(page);
        }

        public static void ProcessRequest(HTCHttpContext context, HttpServerInfo serverInfo) {
            string root = serverInfo.Root;
            string urlPath = context.Request.Path.ToString();
            if (RegisteredPages.ContainsKey(urlPath.ToLower())) {
                if (RegisteredPages[urlPath.ToLower()].OnHttpPageRequest(context, urlPath.ToLower())) {
                    Default5xx.Call500(context);
                }
            } else {
                string requestPath = Path.GetFullPath(Path.Combine(root, urlPath.Remove(0, 1)));
                if (File.Exists(requestPath)) {
                    string extension = Path.GetExtension(requestPath);
                    if (ExtensionPlugins.ContainsKey(extension.ToLower())) {
                        if (ExtensionPlugins[extension.ToLower()].OnHttpExtensionRequest(context, requestPath, extension.ToLower())) {
                            Default5xx.Call500(context);
                        }
                    } else {
                        try {
                            CallFile(context, requestPath);
                        } catch (Exception ex) {
                            _Logger.Error(ex);
                            Default5xx.Call500(context);
                        }
                    }
                } else {
                    if (Directory.Exists(requestPath)) {
                        CallIndex(context, Path.GetFullPath(Path.Combine(root, requestPath)), false);
                    } else {
                        Default4xx.Call404(context);
                    }
                }
            }
            try {
                context.Response.OutputStream.Close();
            } catch { }
        }

        public static void CallIndex(HTCHttpContext context, string serverPath, bool allowDirectoryIndexer) {
            foreach (string indexFile in IndexFiles) {
                string indexPath = Path.Combine(serverPath, indexFile);
                if (RegisteredPages.ContainsKey(indexPath.ToLower())) {
                    if (RegisteredPages[indexFile.ToLower()].OnHttpPageRequest(context, indexPath.ToLower())) {
                        Default5xx.Call500(context);
                    }
                    return;
                } else {
                    if (File.Exists(indexPath)) {
                        IHttpEvents plugin = null;
                        string extension = Path.GetExtension(indexPath);
                        if (ExtensionPlugins.TryGetValue(extension.ToLower(), out plugin)) {
                            if (plugin.OnHttpExtensionRequest(context, indexPath, extension.ToLower())) {
                                Default5xx.Call500(context);
                            }
                        } else {
                            try {
                                CallFile(context, indexPath);
                            } catch (Exception ex) {
                                _Logger.Error(ex);
                                Default5xx.Call500(context);
                            }
                        }
                    }
                }
            }
            if (allowDirectoryIndexer) {
                Default5xx.Call500(context);
            } else {
                Default4xx.Call403(context);
            }
        }

        public static void CallFile(HTCHttpContext context, string requestPath) {
            using (FileBuffer fileBuffer = new FileBuffer(requestPath, 2048)) {
                 ContentType contentType = ContentType.DEFAULT.FromExtension(requestPath);
                // 7 * 24 Hour * 60 Min * 60 Sec = 604800 Sec;
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                context.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(requestPath).ToString("r"));
                context.Response.Headers.Add("Server", "HTCSharp");
                context.Response.Headers.Add("Cache-Control", "max-age=604800");
                context.Response.ContentType = contentType.ToValue();
                context.Response.StatusCode = 200;
                Tuple<long, long> Range = GetRange(context, fileBuffer);
                if (UseGzip(context, fileBuffer.Lenght, contentType)) {
                    context.Response.Headers.Add("Content-Encoding", "gzip");
                    using (GZipStream gzipStream = new GZipStream(context.Response.OutputStream, CompressionMode.Compress, false)) {
                        fileBuffer.CopyToStream(gzipStream, Range.Item1, Range.Item2);
                    }
                } else {
                    context.Response.ContentLength = Range.Item2 - Range.Item1;
                    fileBuffer.CopyToStream(context.Response.OutputStream, Range.Item1, Range.Item2);
                }
            }
        }

        private static bool UseGzip(HTCHttpContext context, long fileLenght, ContentType contentType) {
            string header = GetHeader(context, "Accept-Encoding");
            return fileLenght > (1024 * 5) && fileLenght < (1024 * 1024 * 5) && header != null && header.Contains("gzip", StringComparison.CurrentCultureIgnoreCase);
        }

        private static Tuple<long, long> GetRange(HTCHttpContext context, FileBuffer fileBuffer) {
            string rangeData = GetHeader(context, "Range");
            long startRange = -1;
            long endRange = -1;
            if (rangeData != null) {
                string rangeHeader = rangeData.Replace("bytes=", "");
                string[] range = rangeHeader.Split('-');
                startRange = long.Parse(range[0]);
                if (range[1].Trim().Length > 0) long.TryParse(range[1], out endRange);
                if (endRange == -1) endRange = fileBuffer.Lenght;
            } else {
                startRange = 0;
                endRange = fileBuffer.Lenght;
            }
            return new Tuple<long, long>(startRange, endRange);
        }

        private static string GetHeader(HTCHttpContext context, string name) {
            foreach (string key in context.Request.Headers.Keys) {
                if (key.Equals(name, StringComparison.CurrentCultureIgnoreCase)) {
                    return context.Request.Headers[key];
                }
            }
            return null;
        }
    }
}
