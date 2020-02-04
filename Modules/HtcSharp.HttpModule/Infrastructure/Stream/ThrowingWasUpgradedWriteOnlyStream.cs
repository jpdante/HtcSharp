using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Infrastructure.Stream {
    internal sealed class ThrowingWasUpgradedWriteOnlyStream : WriteOnlyStream {
        public override bool CanSeek => false;

        public override long Length => throw new NotSupportedException();

        public override long Position {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
            => throw new InvalidOperationException("Cannot write to response body after connection has been upgraded.");

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => throw new InvalidOperationException("Cannot write to response body after connection has been upgraded.");

        public override void Flush()
            => throw new InvalidOperationException("Cannot write to response body after connection has been upgraded.");

        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotSupportedException();

        public override void SetLength(long value)
            => throw new NotSupportedException();
    }
}
