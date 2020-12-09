using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Extensions;
using HtcSharp.HttpModule.Routing.Abstractions;
using HtcSharp.HttpModule.Routing.Pages;

namespace HtcSharp.HttpModule.Routing.Directives {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    public class DirectoryListingDirective : IDirective {

        private readonly DirectoryListingTemplate _directoryListingTemplate;
        private readonly bool _enabled;

        public DirectoryListingDirective(string info) {
            if (info.Equals("on", StringComparison.CurrentCultureIgnoreCase)) {
                _directoryListingTemplate = new DirectoryListingTemplate();
                _enabled = true;
            } else if (info.Equals("off", StringComparison.CurrentCultureIgnoreCase)) {
                _enabled = false;
            } else {
                try {
                    string json = File.ReadAllText(info, Encoding.UTF8);
                    _directoryListingTemplate = JsonSerializer.Deserialize<DirectoryListingTemplate>(json);
                    _enabled = _directoryListingTemplate != null;
                } catch {
                    throw new Exception($"Failed to load '{info}' as a template for directory listing.");
                }
            }
        }

        public async Task Execute(HttpContext context) {
            string requestedPath = context.Request.RequestPath.Remove(0, 1);
            string directory = Path.GetFullPath(Path.Combine(context.ServerInfo.RootPath, context.Request.RequestPath.Remove(0, 1)));
            if (!_enabled) return;
            if (Directory.Exists(directory)) {
                var directoryInfo = new DirectoryInfo(directory);
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(_directoryListingTemplate.Header
                    .Replace("%RelativePath%", context.Request.RequestPath)
                );
                if (requestedPath.Length != 0) {
                    await context.Response.WriteAsync(_directoryListingTemplate.DirectoryRow
                        .Replace("%RelativePath%", "..")
                        .Replace("%FileName%", "..")
                    );
                }
                foreach (var fileSystemInfo in directoryInfo.GetFileSystemInfos()) {
                    if (fileSystemInfo.Attributes == FileAttributes.Directory) {
                        await context.Response.WriteAsync(_directoryListingTemplate.DirectoryRow
                            .Replace("%RelativePath%", fileSystemInfo.Name)
                            .Replace("%FileName%", $"{fileSystemInfo.Name}/")
                        );
                    } else {
                        long fileLength = new FileInfo(fileSystemInfo.FullName).Length;
                        await context.Response.WriteAsync(_directoryListingTemplate.FileRow
                            .Replace("%RelativePath%", fileSystemInfo.Name)
                            .Replace("%FileName%", fileSystemInfo.Name)
                            .Replace("%SizeLong%", fileLength.ToString())
                            .Replace("%Size%", FormatSize(fileLength))
                            .Replace("%LastModified%", fileSystemInfo.LastWriteTime.ToString(CultureInfo.InvariantCulture))
                        );
                    }
                }
                stopWatch.Stop();
                await context.Response.WriteAsync(_directoryListingTemplate.Footer.Replace("%RenderTime%", $"{stopWatch.ElapsedMilliseconds} ms"));
                context.Response.HasFinished = true;
            }
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