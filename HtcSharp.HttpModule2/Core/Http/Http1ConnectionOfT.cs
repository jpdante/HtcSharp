using HtcSharp.Core.Models.Http;

namespace HtcSharp.HttpModule2.Core.Http {
    internal sealed class Http1Connection<TContext> : Http1Connection, IHostContextContainer<TContext> {
        public Http1Connection(HttpConnectionContext context) : base(context) { }

        TContext IHostContextContainer<TContext>.HostContext { get; set; }
    }
}
