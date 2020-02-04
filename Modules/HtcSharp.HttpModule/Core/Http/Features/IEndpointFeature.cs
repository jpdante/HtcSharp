using HtcSharp.HttpModule.Http.Http.Abstractions;

namespace HtcSharp.HttpModule.Core.Http.Features {
    /// <summary>
    /// A feature interface for endpoint routing. Use <see cref="HttpContext.Features"/>
    /// to access an instance associated with the current request.
    /// </summary>
    public interface IEndpointFeature {
        /// <summary>
        /// Gets or sets the selected <see cref="HttpModule.Http.Http.Abstractions.Endpoint"/> for the current
        /// request.
        /// </summary>
        Endpoint Endpoint { get; set; }
    }
}
