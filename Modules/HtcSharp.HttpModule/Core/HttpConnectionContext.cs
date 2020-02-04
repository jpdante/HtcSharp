using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using HtcSharp.HttpModule.Core.Http.Features;
using HtcSharp.HttpModule.Core.Infrastructure;
using HtcSharp.HttpModule.Infrastructure.Interfaces;

namespace HtcSharp.HttpModule.Core {
    internal class HttpConnectionContext {
        public string ConnectionId { get; set; }
        public HttpProtocols Protocols { get; set; }
        public ConnectionContext ConnectionContext { get; set; }
        public ServiceContext ServiceContext { get; set; }
        public IFeatureCollection ConnectionFeatures { get; set; }
        public MemoryPool<byte> MemoryPool { get; set; }
        public IPEndPoint LocalEndPoint { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
        public ITimeoutControl TimeoutControl { get; set; }
        public IDuplexPipe Transport { get; set; }
    }
}