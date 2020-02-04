using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Infrastructure.Protocol.Http {
    internal sealed class ZeroContentLengthMessageBody : MessageBody {
        public ZeroContentLengthMessageBody(bool keepAlive)
            : base(null) {
            RequestKeepAlive = keepAlive;
        }

        public override bool IsEmpty => true;

        public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default) => new ValueTask<ReadResult>(new ReadResult(default, isCanceled: false, isCompleted: true));

        public override Task ConsumeAsync() => Task.CompletedTask;

        public override Task StopAsync() => Task.CompletedTask;

        public override void AdvanceTo(SequencePosition consumed) { }

        public override void AdvanceTo(SequencePosition consumed, SequencePosition examined) { }

        public override bool TryRead(out ReadResult result) {
            result = new ReadResult(default, isCanceled: false, isCompleted: true);
            return true;
        }

        public override void Complete(Exception ex) { }

        public override void CancelPendingRead() { }
    }
}