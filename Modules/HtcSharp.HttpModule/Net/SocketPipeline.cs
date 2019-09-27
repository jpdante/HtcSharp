using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Core.Logging;

namespace HtcSharp.HttpModule.Net {
    public class SocketPipeline {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        public readonly int MinimumBufferSize;
        public readonly Socket Socket;

        private readonly Pipe _pipe;

        public SocketPipeline(Socket socket, int minimumBufferSize = 1024) {
            Socket = socket;
            MinimumBufferSize = minimumBufferSize;
            _pipe = new Pipe();
        }

        public async Task FillPipeAsync(Socket socket, PipeWriter writer) {
            while (true) {
                var memory = writer.GetMemory(MinimumBufferSize);
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

        public async Task ReadPipeAsync(PipeReader reader) {
            while (true) {
                var result = await reader.ReadAsync();
                var buffer = result.Buffer;
                SequencePosition? position = null;
                do {
                    position = buffer.PositionOf((byte)'\n');
                    if (position != null) {
                        //ProcessLine(buffer.Slice(0, position.Value));
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
