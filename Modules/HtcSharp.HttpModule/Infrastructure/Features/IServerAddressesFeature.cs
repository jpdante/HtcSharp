using System.Collections.Generic;

namespace HtcSharp.HttpModule.Infrastructure.Features {
    public interface IServerAddressesFeature {
        ICollection<string> Addresses { get; }

        bool PreferHostingUrls { get; set; }
    }
}
