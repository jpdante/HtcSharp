using System;
using System.IO;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Core.Templates {
    public class FileTemplate : ITemplate, IDisposable {

        public bool SupportGetString => false;
        public string Path { get; }
        public string ContentType { get; }

        private readonly FileStream _fileStream;

        public FileTemplate(string path, string contentType) {
            Path = path;
            ContentType = contentType;
            _fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public FileTemplate(string path, ContentType contentType) : this(path, contentType.ToValue()) {

        }

        public Task<string> GetString() {
            throw new NotSupportedException();
        }

        public Task<string> GetReplaced(HtcHttpContext httpContext) {
            throw new NotSupportedException();
        }

        public async Task SendTemplate(HtcHttpContext httpContext) {
            httpContext.Response.ContentType = ContentType;
            await _fileStream.CopyToAsync(httpContext.Response.Body);
        }

        public void Dispose() {
            _fileStream?.Dispose();
        }
    }
}