using System;

namespace HtcSharp.HttpModule.Core.Http.Features {
    public interface IServiceProvidersFeature {
        IServiceProvider RequestServices { get; set; }
    }
}