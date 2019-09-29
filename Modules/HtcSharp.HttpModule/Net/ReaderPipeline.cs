using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Core.Logging;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Interface;

namespace HtcSharp.HttpModule.Net {
    public class ReaderPipeline {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        public readonly int MinimumBufferSize;
        public readonly HttpParser Parser;

        public ReaderPipeline(HttpParser httpParser, int minimumBufferSize = 1024) {
            Parser = httpParser;
            MinimumBufferSize = minimumBufferSize;
        }

        public Task ProcessLinesAsync(Socket socket, IParserRequestHandler handler) {
            var pipe = new Pipe();
            var writing = FillPipeAsync(socket, pipe.Writer);
            var reading = ReadPipeAsync(pipe.Reader, handler);
            return Task.WhenAll(reading, writing);
        }

        public async Task FillPipeAsync(Socket socket, PipeWriter writer) {
            while (true) {
                var memory = writer.GetMemory(1024);
                try {
                    var bytesRead = await socket.ReceiveAsync(memory, SocketFlags.None);
                    if (bytesRead == 0) break;
                    writer.Advance(bytesRead);
                }
                catch (Exception ex) {
                    Logger.Error(ex);
                    break;
                }
                var result = await writer.FlushAsync();
                if (result.IsCompleted) break;
            }
            writer.Complete();
        }

        public async Task ReadPipeAsync(PipeReader reader, IParserRequestHandler handler) {
            while (true) {
                var result = await reader.ReadAsync();
                var buffer = result.Buffer;
                SequencePosition? position = null;
                do {
                    position = buffer.PositionOf((byte)'\n');
                    if (position != null) {
                        var sequence = buffer.Slice(0, position.Value);
                        Parser.ParseRequestLine(handler, sequence, out var consumed, out var examined);
                        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                    }
                }
                while (position != null);
                reader.AdvanceTo(buffer.Start, buffer.End);
                if (result.IsCompleted) break;
            }
            reader.Complete();
        }

    }
}
