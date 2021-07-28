using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Routing.Directives.Internal;
using Microsoft.AspNetCore.Http;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class ListDirectoryDirective : IDirective {

        private readonly DirectiveDelegate _next;

        private readonly DirectoryListingTemplate _templete;

        public ListDirectoryDirective(DirectiveDelegate next, JsonElement config) {
            _next = next;
            _templete = new DirectoryListingTemplate();
        }

        public async Task Invoke(HtcHttpContext httpContext) {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var directoryContents = httpContext.Site.FileProvider.GetDirectoryContents(httpContext.Request.Path);
            if (directoryContents.Exists) {
                httpContext.Response.StatusCode = 200;
                await httpContext.Response.WriteAsync(_templete.Header.Replace("%RelativePath%", httpContext.Request.Path));
                if (httpContext.Request.Path.Value.Length > 1) {
                    await httpContext.Response.WriteAsync(_templete.DirectoryRow
                        .Replace("%RelativePath%", "..")
                        .Replace("%FileName%", "..")
                    );
                }
                foreach (var fileInfo in directoryContents) {
                    if (fileInfo.IsDirectory) {
                        await httpContext.Response.WriteAsync(_templete.DirectoryRow
                            .Replace("%RelativePath%", fileInfo.Name)
                            .Replace("%FileName%", $"{fileInfo.Name}/")
                        );
                    } else {
                        await httpContext.Response.WriteAsync(_templete.FileRow
                            .Replace("%RelativePath%", fileInfo.Name)
                            .Replace("%FileName%", fileInfo.Name)
                            .Replace("%SizeLong%", fileInfo.Length.ToString())
                            .Replace("%Size%", FormatSize(fileInfo.Length))
                            .Replace("%LastModified%", fileInfo.LastModified.ToString(CultureInfo.InvariantCulture))
                        );
                    }
                }
                stopWatch.Stop();
                await httpContext.Response.WriteAsync(_templete.Footer.Replace("%RenderTime%", $"{stopWatch.ElapsedMilliseconds} ms"));
                return;
            }
            await _next(httpContext);
        }

        private static readonly string[] Suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public static string FormatSize(long bytes) {
            var counter = 0;
            var number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1) {
                number /= 1024;
                counter++;
            }
            return $"{number:n1} {Suffixes[counter]}";
        }
    }
}