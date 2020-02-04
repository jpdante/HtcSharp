using System;
using System.Buffers;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http {
    public interface IHttpParser<TRequestHandler> where TRequestHandler : IHttpHeadersHandler, IHttpRequestLineHandler {
        bool ParseRequestLine(TRequestHandler handler, in ReadOnlySequence<byte> buffer, out SequencePosition consumed, out SequencePosition examined);

        bool ParseHeaders(TRequestHandler handler, ref SequenceReader<byte> reader);
    }
}
