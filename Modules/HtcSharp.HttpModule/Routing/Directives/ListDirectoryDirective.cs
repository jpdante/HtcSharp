using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;
using Microsoft.AspNetCore.Http;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class ListDirectoryDirective : IDirective {
        private readonly DirectiveDelegate _next;

        public ListDirectoryDirective(DirectiveDelegate next, JsonElement config) {
            _next = next;
        }

        public async Task Invoke(HtcHttpContext httpContext) {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var directoryContents = httpContext.Site.FileProvider.GetDirectoryContents(httpContext.Request.Path.Value);
            if (directoryContents.Exists) {
                string headerTemplate = await httpContext.Site.TemplateManager.GetTemplate("DirListHeader").GetString();
                string fileRowTemplate = await httpContext.Site.TemplateManager.GetTemplate("DirListFRow").GetString();
                string dirRowTemplate = await httpContext.Site.TemplateManager.GetTemplate("DirListDRow").GetString();
                string footerTemplate = await httpContext.Site.TemplateManager.GetTemplate("DirListFooter").GetString();

                httpContext.Response.StatusCode = 200;
                await httpContext.Response.WriteAsync(headerTemplate.Replace("$RelativePath", httpContext.Request.Path.Value));
                if (httpContext.Request.Path.Value.Length > 1) {
                    await httpContext.Response.WriteAsync(
                        fileRowTemplate
                            .Replace("$RelativePath", "..")
                            .Replace("$FileName", "..")
                            .Replace("$Size", "-")
                            .Replace("$SizeLong", "-1")
                            .Replace("$LastModified", "-")
                    );
                }

                foreach (var fileInfo in directoryContents) {
                    if (fileInfo.IsDirectory) {
                        await httpContext.Response.WriteAsync(dirRowTemplate
                            .Replace("$RelativePath", $"{fileInfo.Name}/")
                            .Replace("$FileName", $"{fileInfo.Name}/")
                            .Replace("$SizeLong", "-1")
                            .Replace("$Size", "-")
                            .Replace("$LastModified", fileInfo.LastModified.DateTime.ToString("dd/MM/yyy HH:mm:ss", CultureInfo.InvariantCulture))
                        );
                    } else {
                        await httpContext.Response.WriteAsync(fileRowTemplate
                            .Replace("$RelativePath", $"{fileInfo.Name}")
                            .Replace("$FileName", fileInfo.Name)
                            .Replace("$SizeLong", fileInfo.Length.ToString())
                            .Replace("$Size", FormatSize(fileInfo.Length))
                            .Replace("$LastModified", fileInfo.LastModified.DateTime.ToString("dd/MM/yyy HH:mm:ss", CultureInfo.InvariantCulture))
                        );
                    }
                }

                stopWatch.Stop();
                await httpContext.Response.WriteAsync(footerTemplate.Replace("$RenderTime", $"{stopWatch.ElapsedMilliseconds} ms"));
                return;
            }

            await _next(httpContext);
        }

        private static readonly string[] Suffixes = {"Bytes", "KB", "MB", "GB", "TB", "PB"};

        public static string FormatSize(long bytes) {
            var counter = 0;
            var number = (decimal) bytes;
            while (Math.Round(number / 1024) >= 1) {
                number /= 1024;
                counter++;
            }

            return $"{number:n1} {Suffixes[counter]}";
        }
    }
}