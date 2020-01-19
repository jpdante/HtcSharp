namespace HtcSharp.HttpModule.Core.Http.Features {
    public interface IHttpResponseTrailersFeature {
        IHeaderDictionary Trailers { get; set; }
    }
}
