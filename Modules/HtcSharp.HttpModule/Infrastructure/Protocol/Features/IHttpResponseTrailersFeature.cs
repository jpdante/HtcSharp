namespace HtcSharp.HttpModule.Infrastructure.Protocol.Features {
    public interface IHttpResponseTrailersFeature {
        IHeaderDictionary Trailers { get; set; }
    }
}
