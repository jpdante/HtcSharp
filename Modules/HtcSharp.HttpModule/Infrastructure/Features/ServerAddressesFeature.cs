using System.Collections.Generic;

namespace HtcSharp.HttpModule.Infrastructure.Features {
    internal class ServerAddressesFeature : IServerAddressesFeature {
        public ICollection<string> Addresses { get; } = new List<string>();
        public bool PreferHostingUrls { get; set; }
    }
}