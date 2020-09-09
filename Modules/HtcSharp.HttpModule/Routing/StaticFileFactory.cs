using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Middleware.StaticFiles;

namespace HtcSharp.HttpModule.Routing {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    public class StaticFileFactory {

        private readonly StaticFileOptions _staticFileOptions;
        private readonly IContentTypeProvider _contentTypeProvider;

        public StaticFileFactory(StaticFileOptions staticFileOptions, IContentTypeProvider contentTypeProvider) {
            _staticFileOptions = staticFileOptions;
            _contentTypeProvider = contentTypeProvider;
        }

        private void LookupContentType(StaticFileOptions options, string filePath, out string contentType) {
            if (!_contentTypeProvider.TryGetContentType(filePath, out contentType)) {
                contentType = options.DefaultContentType;
            }
        }

        public async Task ServeStaticFile(HttpContext context, string filePath) {
            LookupContentType(_staticFileOptions, filePath, out string contentType);
            var fileContext = new StaticFileContext(context, _staticFileOptions, filePath, contentType);
            if (fileContext.LookupFileInfo()) {
                await fileContext.ServeStaticFile();
            }
        }
    }
}
