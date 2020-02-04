using HtcSharp.HttpModule.Infrastructure.Http.Abstractions;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http {
    internal sealed class Http1Connection<TContext> : Http1Connection, IHostContextContainer<TContext> {
        public Http1Connection(HttpConnectionContext context) : base(context) { }

        TContext IHostContextContainer<TContext>.HostContext { get; set; }
    }
}
