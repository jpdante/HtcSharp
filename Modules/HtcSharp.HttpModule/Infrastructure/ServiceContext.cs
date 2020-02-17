using System.IO.Pipelines;
using HtcSharp.HttpModule.Core.Http.Http;
using HtcSharp.HttpModule.Core.Infrastructure;
using HtcSharp.HttpModule.Infrastructure.Heart;
using HtcSharp.HttpModule.Infrastructure.Interfaces;
using HtcSharp.HttpModule.Infrastructure.Options;

namespace HtcSharp.HttpModule.Infrastructure {
    internal class ServiceContext {
        public IKestrelTrace Log { get; set; }

        public PipeScheduler Scheduler { get; set; }

        public IHttpParser<Http1ParsingHandler> HttpParser { get; set; }

        public ISystemClock SystemClock { get; set; }

        public DateHeaderValueManager DateHeaderValueManager { get; set; }

        public ConnectionManager ConnectionManager { get; set; }

        public Heartbeat Heartbeat { get; set; }

        public KestrelServerOptions ServerOptions { get; set; }
    }
}