using Microsoft.Extensions.FileProviders;

namespace HtcSharp.HttpModule.Abstractions {
    public interface IReadOnlySite {

        public IFileProvider FileProvider { get; }

    }
}