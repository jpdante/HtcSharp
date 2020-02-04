namespace HtcSharp.HttpModule.Infrastructure.Features {
    public interface IHttpResponseTrailersFeature {
        IHeaderDictionary Trailers { get; set; }
    }
}
