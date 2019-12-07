using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.Core.Logging;
using HtcSharp.HttpModule2.Core;

namespace HtcSharp.HttpModule2.Connection.Address {
    internal class AddressBindContext {
        public ICollection<string> Addresses { get; set; }
        public List<ListenOptions.ListenOptions> ListenOptions { get; set; }
        public KestrelServerOptions ServerOptions { get; set; }
        public Logger Logger { get; set; }

        public Func<ListenOptions.ListenOptions, Task> CreateBinding { get; set; }
    }
}