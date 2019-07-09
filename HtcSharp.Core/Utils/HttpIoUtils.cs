using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.IO.Http;
using HtcSharp.Core.Models.Http;

namespace HtcSharp.Core.Utils {
    public static class HttpIoUtils {
        public static void CallFile(HtcHttpContext httpContext, string requestPath) {
            using (var fileBuffer = new FileBuffer(requestPath, 2048)) {
                var contentType = ContentType.DEFAULT.FromExtension(requestPath);
                // 7 * 24 Hour * 60 Min * 60 Sec = 604800 Sec;
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                httpContext.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                //context.Response.Headers.Add("Last-Modified", File.GetLastWriteTime(requestPath).ToString("r"));
                httpContext.Response.Headers.Add("Server", "HtcSharp");
                //context.Response.Headers.Add("Cache-Control", "max-age=604800");
                httpContext.Response.ContentType = contentType.ToValue();
                httpContext.Response.StatusCode = 200;
                var (startRange, endRange) = GetRange(httpContext, fileBuffer);
                if (UseGzip(httpContext, fileBuffer.Lenght)) {
                    httpContext.Response.Headers.Add("Content-Encoding", "gzip");
                    using (var gzipStream = new GZipStream(httpContext.Response.OutputStream, CompressionMode.Compress, false)) {
                        fileBuffer.CopyToStream(gzipStream, startRange, endRange);
                    }
                } else {
                    httpContext.Response.ContentLength = endRange - startRange;
                    fileBuffer.CopyToStream(httpContext.Response.OutputStream, startRange, endRange);
                }
            }
        }

        private static bool UseGzip(HtcHttpContext httpContext, long fileSize) {
            var header = GetHeader(httpContext, "Accept-Encoding");
            return fileSize > (1024 * 5) && fileSize < (1024 * 1024 * 5) && header != null && header.Contains("gzip", StringComparison.CurrentCultureIgnoreCase);
        }

        private static Tuple<long, long> GetRange(HtcHttpContext httpContext, FileBuffer fileBuffer) {
            var rangeData = GetHeader(httpContext, "Range");
            long startRange;
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
