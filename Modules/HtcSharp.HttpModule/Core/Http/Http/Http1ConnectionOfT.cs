using HtcSharp.HttpModule.Http.Http.Abstractions;

namespace HtcSharp.HttpModule.Core.Http.Http {
    internal sealed class Http1Connection<TContext> : Http1Connection, IHostContextContainer<TContext> {
        public Http1Connection(HttpConnectionContext context) : base(context) { }

        TContext IHostContextContainer<TContext>.HostContext { get; set; }
    }
}
