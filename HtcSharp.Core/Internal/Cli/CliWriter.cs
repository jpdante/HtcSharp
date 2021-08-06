using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.Abstractions.Cli;

namespace HtcSharp.Core.Internal.Cli {
    public class CliWriter : IWriter, IAsyncDisposable, IDisposable {

        private readonly StreamWriter _streamWriter;

        public CliWriter(Stream stream) : this(stream, Encoding.UTF8) { }

        public CliWriter(Stream stream, Encoding encoding, int bufferSize = -1, bool leaveOpen = true) {
            _streamWriter = new StreamWriter(stream, encoding, bufferSize, leaveOpen);
        }

        public void Write(ReadOnlySpan<char> value) => _streamWriter.Write(value);
        public void Write(StringBuilder value) => _streamWriter.Write(value);
        public void Write(string value) => _streamWriter.Write(value);
        public void Write(char value) => _streamWriter.Write(value);
        public void Write(char[] buffer, int index, int count) => _streamWriter.Write(buffer, index, count);

        public void WriteLine(ReadOnlySpan<char> value) => _streamWriter.WriteLine(value);
        public void WriteLine(StringBuilder value) => _streamWriter.WriteLine(value);
        public void WriteLine(string value) => _streamWriter.WriteLine(value);
        public void WriteLine(char value) => _streamWriter.WriteLine(value);

        public Task WriteAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken = default) => _streamWriter.WriteAsync(value, cancellationToken);
        public Task WriteAsync(StringBuilder value, CancellationToken cancellationToken = default)  => _streamWriter.WriteAsync(value, cancellationToken);
        public Task WriteAsync(string value) => _streamWriter.WriteAsync(value);
        public Task WriteAsync(char value)  => _streamWriter.WriteAsync(value);
        public Task WriteAsync(char[] buffer, int index, int count)  => _streamWriter.WriteAsync(buffer, index, count);

        public Task WriteLineAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken = default) => _streamWriter.WriteLineAsync(value, cancellationToken);
        public Task WriteLineAsync(StringBuilder value, CancellationToken cancellationToken = default) => _streamWriter.WriteLineAsync(value, cancellationToken);
        public Task WriteLineAsync(string value) => _streamWriter.WriteLineAsync(value);
        public Task WriteLineAsync(char value) => _streamWriter.WriteLineAsync(value);

        public void Flush() => _streamWriter.Flush();

        public Task FlushAsync() => _streamWriter.FlushAsync();

        public void Dispose() => _streamWriter.Dispose();

        public ValueTask DisposeAsync() => _streamWriter.DisposeAsync();
    }
}