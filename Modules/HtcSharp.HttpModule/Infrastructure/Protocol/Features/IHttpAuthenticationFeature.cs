using System.Security.Claims;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Features {
    public interface IHttpAuthenticationFeature {
        ClaimsPrincipal User { get; set; }
    }
}
