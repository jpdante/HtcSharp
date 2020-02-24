using System;
using System.IO.Compression;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;

namespace HtcSharp.HttpModule.IO {
    public static class HttpIO {
        public static async Task SendFile(HttpContext httpContext, string requestPath) {
            //httpContext.Response.

            /*using (var fileBuffer = new FileBuffer(requestPath, 2048)) {
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
                    using var gzipStream = new GZipStream(httpContext.Response.OutputStream, CompressionLevel.Fastest, false);
                    fileBuffer.CopyToStream(gzipStream, startRange, endRange);
                } else {
                    httpContext.Response.ContentLength = endRange - startRange;
                    fileBuffer.CopyToStream(httpContext.Response.OutputStream, startRange, endRange);
                }
            }*/
        }

        /*private static bool UseGzip(HttpContext httpContext, long fileSize) {
            var header = GetHeader(httpContext, "Accept-Encoding");
            return fileSize > (1024 * 5) && fileSize < (1024 * 1024 * 5) && header != null && header.Contains("gzip", StringComparison.CurrentCultureIgnoreCase);
        }

        private static Tuple<long, long> GetRange(HttpContext httpContext, FileBuffer fileBuffer) {
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

        private static string GetHeader(HttpContext httpContext, string name) {
            foreach (var key in httpContext.Request.Headers.Keys) {
                if (key.Equals(name, StringComparison.CurrentCultureIgnoreCase)) {
                    return httpContext.Request.Headers[key];
                }
            }
            return null;
        }*/

        public static string ReplaceVars(HttpContext httpContext, string data) {
            data = data.Replace("$scheme", httpContext.Request.Scheme);
            data = data.Replace("$uri", httpContext.Request.RequestPath);
            data = data.Replace("$host", httpContext.Request.Host.ToString());
            return data;
        }
    }
}
