using System;

namespace HtcSharp.HttpModule.Infrastructure.Features {
    public interface IServiceProvidersFeature {
        IServiceProvider RequestServices { get; set; }
    }
}