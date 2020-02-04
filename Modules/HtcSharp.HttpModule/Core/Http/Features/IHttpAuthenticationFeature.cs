using System.Security.Claims;

namespace HtcSharp.HttpModule.Core.Http.Features {
    public interface IHttpAuthenticationFeature {
        ClaimsPrincipal User { get; set; }
    }
}
