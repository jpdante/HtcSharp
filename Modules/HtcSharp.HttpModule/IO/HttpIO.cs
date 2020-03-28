using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using HtcSharp.HttpModule.Shared;

namespace HtcSharp.HttpModule.IO {
    public static class HttpIO {
        public static async Task SendFile(HttpContext httpContext, string requestPath) {
            await using var fileStream = new FileStream(requestPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            GetTransferPosition(httpContext, fileStream.Length, out long startRange, out long endRange);
            fileStream.Position = startRange;
            var contentType = ContentType.DEFAULT.FromExtension(requestPath);
            // 7 * 24 Hour * 60 Min * 60 Sec = 604800 Sec;
            //httpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            httpContext.Response.Headers["Date"] = DateTime.Now.ToString("r");
            //context.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(requestPath).ToString("r"));
            httpContext.Response.Headers["Server"] = "HtcSharp";
            //context.Response.Headers.Add("Cache-Control", "max-age=604800");
            httpContext.Response.ContentType = contentType.ToValue();
            httpContext.Response.StatusCode = 200;
            if (UseGzip(httpContext, fileStream.Length)) {
                httpContext.Response.Headers["Content-Encoding"] = "gzip";
                await using var gzipStream = new GZipStream(httpContext.Response.Body, CompressionLevel.Fastest);
                await StreamCopyOperationInternal.CopyToAsync(fileStream, gzipStream, endRange, httpContext.RequestAborted);
            } else {
                httpContext.Response.ContentLength = endRange - startRange;
                await StreamCopyOperationInternal.CopyToAsync(fileStream, httpContext.Response.Body, endRange, httpContext.RequestAborted);
            }
        }

        private static void GetTransferPosition(HttpContext httpContext, long length, out long startRange, out long endRange) {
            string rangeData = GetHeader(httpContext, "Range");
            endRange = -1;
            if (rangeData != null) {
                string rangeHeader = rangeData.Replace("bytes=", "");
                string[] range = rangeHeader.Split('-');
                startRange = long.Parse(range[0]);
                if (startRange >= length) {
                    startRange = length;
                    endRange = 0;
                    return;
                }
                if (range[1].Trim().Length > 0) long.TryParse(range[1], out endRange);
                if (endRange >= length) {
                    startRange = length;
                    endRange = 0;
                    return;
                }
                if (endRange == -1) endRange = length;
            } else {
                startRange = 0;
                endRange = length;
            }
        }

        private static string GetHeader(HttpContext httpContext, string name) {
            foreach (string key in httpContext.Request.Headers.Keys) {
                if (key.Equals(name, StringComparison.CurrentCultureIgnoreCase)) {
                    return httpContext.Request.Headers[key];
                }
            }
            return null;
        }

        private static bool UseGzip(HttpContext httpContext, long fileSize) {
            string header = GetHeader(httpContext, "Accept-Encoding");
            return fileSize > (1024 * 5) && fileSize < (1024 * 1024 * 5) && header != null && header.Contains("gzip", StringComparison.CurrentCultureIgnoreCase);
        }

        public static string ReplaceVars(HttpContext httpContext, string data) {
            data = data.Replace("$scheme", httpContext.Request.Scheme);
            data = data.Replace("$uri", httpContext.Request.RequestPath);
            data = data.Replace("$host", httpContext.Request.Host.ToString());
            return data;
        }
    }
}
