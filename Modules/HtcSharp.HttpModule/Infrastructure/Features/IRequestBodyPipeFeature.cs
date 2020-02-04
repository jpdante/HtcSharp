using System.IO.Pipelines;

namespace HtcSharp.HttpModule.Infrastructure.Features {
    /// <summary>
    /// Represents the HttpRequestBody as a PipeReader.
    /// </summary>
    public interface IRequestBodyPipeFeature
    {
        /// <summary>
        /// A <see cref="PipeReader"/> representing the request body, if any.
        /// </summary>
        PipeReader Reader { get; }
    }
}
