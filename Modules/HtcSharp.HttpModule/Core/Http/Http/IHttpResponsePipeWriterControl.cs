using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Core.Http.Http {
    internal interface IHttpResponsePipeWriterControl {
        void ProduceContinue();
        Memory<byte> GetMemory(int sizeHint = 0);
        Span<byte> GetSpan(int sizeHint = 0);
        void Advance(int bytes);
        ValueTask<FlushResult> FlushPipeAsync(CancellationToken cancellationToken);
        ValueTask<FlushResult> WritePipeAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken);
        void CancelPendingFlush();
    }
}
