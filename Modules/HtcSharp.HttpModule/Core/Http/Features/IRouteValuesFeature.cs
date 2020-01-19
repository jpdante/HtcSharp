using HtcSharp.HttpModule.Http.Http.Abstractions;

namespace HtcSharp.HttpModule.Core.Http.Features {
    /// <summary>
    /// A feature interface for routing values. Use <see cref="HttpContext.Features"/>
    /// to access the values associated with the current request.
    /// </summary>
    public interface IRouteValuesFeature {
        /// <summary>
        /// Gets or sets the <see cref="RouteValueDictionary"/> associated with the currrent
        /// request.
        /// </summary>
        RouteValueDictionary RouteValues { get; set; }
    }
}