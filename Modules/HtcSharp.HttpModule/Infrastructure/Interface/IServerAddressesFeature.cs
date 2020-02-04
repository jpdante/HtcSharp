using System.Collections.Generic;

namespace HtcSharp.HttpModule.Infrastructure.Interface {
    public interface IServerAddressesFeature {
        ICollection<string> Addresses { get; }

        bool PreferHostingUrls { get; set; }
    }
}