using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Infrastructure.Options;
using Microsoft.Extensions.Logging;

namespace HtcSharp.HttpModule.Infrastructure.Binding {
    internal class AddressBindContext {
        public ICollection<string> Addresses { get; set; }
        public List<ListenOptions> ListenOptions { get; set; }
        public KestrelServerOptions ServerOptions { get; set; }
        public ILogger Logger { get; set; }

        public Func<ListenOptions, Task> CreateBinding { get; set; }
    }
}