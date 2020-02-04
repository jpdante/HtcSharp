using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using HtcSharp.HttpModule.Infrastructure.Interface;
using HtcSharp.HttpModule.Infrastructure.Protocol.Features;

namespace HtcSharp.HttpModule.Infrastructure {
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