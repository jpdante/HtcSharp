using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.Abstractions.Cli {
    public interface IWriter {

        public void Write(ReadOnlySpan<char> value);
        public void Write(StringBuilder value);
        public void Write(string value);
        public void Write(char value);
        public void Write(char[] buffer, int index, int count);

        public void WriteLine(ReadOnlySpan<char> value);
        public void WriteLine(StringBuilder value);
        public void WriteLine(string value);
        public void WriteLine(char value);

        public Task WriteAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken);
        public Task WriteAsync(StringBuilder value, CancellationToken cancellationToken);
        public Task WriteAsync(string value);
        public Task WriteAsync(char value);
        public Task WriteAsync(char[] buffer, int index, int count);

        public Task WriteLineAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken);
        public Task WriteLineAsync(StringBuilder value, CancellationToken cancellationToken);
        public Task WriteLineAsync(string value);
        public Task WriteLineAsync(char value);

    }
}