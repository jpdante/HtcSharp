using System;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Features {
    public interface IServiceProvidersFeature {
        IServiceProvider RequestServices { get; set; }
    }
}