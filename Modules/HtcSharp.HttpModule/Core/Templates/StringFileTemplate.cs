using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Http;
using Microsoft.AspNetCore.Http;

namespace HtcSharp.HttpModule.Core.Templates {
    public class StringFileTemplate : ITemplate, IDisposable {

        public bool SupportGetString => true;
        public string Path { get; }
        public string ContentType { get; }
        public bool ShouldCache { get; }
        public bool WatchFileChanges { get; }

        private string _cache;

        private readonly FileStream _fileStream;
        private readonly StreamReader _streamReader;
        private readonly FileSystemWatcher _fileSystemWatcher;

        public StringFileTemplate(string path, string contentType, bool shouldCache = true, bool watchFileChanges = true) {
            Path = path;
            ContentType = contentType;
            ShouldCache = shouldCache;
            WatchFileChanges = watchFileChanges;

            if (ShouldCache && WatchFileChanges) {
                _fileSystemWatcher = new FileSystemWatcher {
                    Path = Path,
                    EnableRaisingEvents = true,
                };
                _fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
                using var fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var streamReader = new StreamReader(_fileStream);
                _cache = streamReader.ReadToEnd();
            } else if (ShouldCache && !WatchFileChanges) {
                using var fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var streamReader = new StreamReader(_fileStream);
                _cache = streamReader.ReadToEnd();
            } else if (!ShouldCache) {
                _fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                _streamReader = new StreamReader(_fileStream);
            }
        }

        public StringFileTemplate(string path, ContentType contentType, bool shouldCache = true, bool watchFileChanges = true) : this(path, contentType.ToValue(), shouldCache, watchFileChanges) {

        }

        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs e) {
            try {
                using var fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var streamReader = new StreamReader(_fileStream);
                _cache = streamReader.ReadToEnd();
            } catch (Exception) {
                _cache = "Failed to open file.";
            }
        }

        public async Task<string> GetString() {
            return ShouldCache ? _cache : await _streamReader.ReadToEndAsync();
        }

        public async Task<string> GetReplaced(HtcHttpContext httpContext) {
            var dataBuilder = ShouldCache ? new StringBuilder(_cache) : new StringBuilder(await _streamReader.ReadToEndAsync());
            dataBuilder.Replace("$Scheme", httpContext.Request.Scheme);
            dataBuilder.Replace("$Path", httpContext.Request.Path.Value);
            dataBuilder.Replace("$ContentType", httpContext.Request.ContentType);
            dataBuilder.Replace("$Method", httpContext.Request.Method);
            dataBuilder.Replace("$Host", httpContext.Request.Host.Value);
            dataBuilder.Replace("$RemoteAddr", httpContext.Connection.RemoteIpAddress.ToString());
            dataBuilder.Replace("$RemotePort", httpContext.Connection.RemotePort.ToString());
            dataBuilder.Replace("$IsHttps", httpContext.Request.IsHttps.ToString());
            return dataBuilder.ToString();
        }

        public async Task SendTemplate(HtcHttpContext httpContext) {
            httpContext.Response.ContentType = ContentType;
            await httpContext.Response.WriteAsync(await GetReplaced(httpContext));
        }

        public void Dispose() {
            _streamReader?.Dispose();
            _fileStream?.Dispose();
            _fileSystemWatcher?.Dispose();
        }
    }
}