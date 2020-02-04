using System.Security.Claims;

namespace HtcSharp.HttpModule.Infrastructure.Features {
    public interface IHttpAuthenticationFeature {
        ClaimsPrincipal User { get; set; }
    }
}
