using HtcSharp.HttpModule.Infrastructure.Http.Abstractions;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http2 {
    internal sealed class Http2Stream<TContext> : Http2Stream, IHostContextContainer<TContext> {
        private readonly IHttpApplication<TContext> _application;

        public Http2Stream(IHttpApplication<TContext> application, Http2StreamContext context) : base(context) {
            _application = application;
        }

        public override void Execute() {
            // REVIEW: Should we store this in a field for easy debugging?
            _ = ProcessRequestsAsync(_application);
        }

        // Pooled Host context
        TContext IHostContextContainer<TContext>.HostContext { get; set; }
    }
}
