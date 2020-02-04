namespace HtcSharp.HttpModule.Infrastructure.Features {
    /// <summary>
    /// Feature to identify a request.
    /// </summary>
    public interface IHttpRequestIdentifierFeature {
        /// <summary>
        /// Identifier to trace a request.
        /// </summary>
        string TraceIdentifier { get; set; }
    }
}
