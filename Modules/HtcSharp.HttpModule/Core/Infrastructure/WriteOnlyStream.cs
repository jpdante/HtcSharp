using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Core.Infrastructure {
    internal abstract class WriteOnlyStream : Stream {
        public override bool CanRead => false;

        public override bool CanWrite => true;

        public override int ReadTimeout {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => throw new NotSupportedException();
    }
}