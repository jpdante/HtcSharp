#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.Abstractions.Cli {
    public interface IReader {

        public int Read();
        public int Read(Span<char> buffer);
        public int Read(char[] buffer, int index, int count);

        public ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken);
        public Task<int> ReadAsync(char[] buffer, int index, int count);

        public int ReadBlock(Span<char> buffer);
        public int ReadBlock(char[] buffer, int index, int count);

        public ValueTask<int> ReadBlockAsync(Memory<char> buffer, CancellationToken cancellationToken);
        public Task<int> ReadBlockAsync(char[] buffer, int index, int count);

        public string? ReadLine();

        public Task<string?> ReadLineAsync();

        public string ReadToEnd();

        public Task<string> ReadToEndAsync();

    }
}