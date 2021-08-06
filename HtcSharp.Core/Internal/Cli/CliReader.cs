using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.Abstractions.Cli;

namespace HtcSharp.Core.Internal.Cli {
    public class CliReader : IReader, IDisposable {

        private readonly StreamReader _streamReader;

        public CliReader(Stream stream) : this(stream, Encoding.UTF8) { }

        public CliReader(Stream stream, Encoding encoding, int bufferSize = -1, bool leaveOpen = true) {
            _streamReader = new StreamReader(stream, encoding, bufferSize: bufferSize, leaveOpen: leaveOpen);
        }

        public int Read() => _streamReader.Read();
        public int Read(Span<char> buffer) => _streamReader.Read(buffer);
        public int Read(char[] buffer, int index, int count) => _streamReader.Read(buffer, index, count);

        public ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default) => _streamReader.ReadAsync(buffer, cancellationToken);
        public Task<int> ReadAsync(char[] buffer, int index, int count) => _streamReader.ReadAsync(buffer, index, count);

        public int ReadBlock(Span<char> buffer) => _streamReader.ReadBlock(buffer);
        public int ReadBlock(char[] buffer, int index, int count) => _streamReader.ReadBlock(buffer, index, count);

        public ValueTask<int> ReadBlockAsync(Memory<char> buffer, CancellationToken cancellationToken = default) => _streamReader.ReadBlockAsync(buffer, cancellationToken);
        public Task<int> ReadBlockAsync(char[] buffer, int index, int count) => _streamReader.ReadBlockAsync(buffer, index, count);

        public string? ReadLine() => _streamReader.ReadLine();

        public Task<string?> ReadLineAsync() => _streamReader.ReadLineAsync();

        public string ReadToEnd() => _streamReader.ReadToEnd();

        public Task<string> ReadToEndAsync() => _streamReader.ReadToEndAsync();

        public void Dispose() {
            _streamReader.Dispose();
        }
    }
}