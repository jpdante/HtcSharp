using System.IO.Pipelines;
using HtcSharp.HttpModule.Core;
using HtcSharp.HttpModule.Core.Http.Http;
using HtcSharp.HttpModule.Core.Infrastructure;
using HtcSharp.HttpModule.Infrastructure;
using HtcSharp.HttpModule.Infrastructure.BindOptions;
using HtcSharp.HttpModule.Infrastructure.HeartBeat;
using HtcSharp.HttpModule.Infrastructure.Interface;
using HtcSharp.HttpModule.Infrastructure.Protocol.Http;

namespace HtcSharp.HttpModule {
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