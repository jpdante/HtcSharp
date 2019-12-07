using System.Collections.Generic;

namespace HtcSharp.HttpModule2.Core {
    public interface IServerAddressesFeature {
        ICollection<string> Addresses { get; }

        bool PreferHostingUrls { get; set; }
    }
}
