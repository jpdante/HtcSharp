using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.Cli.Core {
    public class CliClient : IDisposable {

        public string ServerName { get; }
        public string PipeName { get; }

        private NamedPipeClientStream _namedPipeClientStream;
        private CancellationTokenSource _cancellationTokenSource;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;

        public CliClient(string serverName, string pipeName) {
            ServerName = serverName;
            PipeName = pipeName;
        }

        public async Task Start() {
            _namedPipeClientStream = new NamedPipeClientStream(ServerName, PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            _cancellationTokenSource = new CancellationTokenSource();
            await _namedPipeClientStream.ConnectAsync();
            _streamReader = new StreamReader(_namedPipeClientStream, Encoding.UTF8);
            _streamWriter = new StreamWriter(_namedPipeClientStream, Encoding.UTF8);
        }

        public async Task Stop() {
            _cancellationTokenSource.Cancel();
            await _namedPipeClientStream.DisposeAsync();
        }

        public async Task SendCommand(string command) {
            await _streamWriter.WriteLineAsync(command);
            await _streamWriter.FlushAsync();
            await Reader();
        }

        public async Task Reader() {
            try {
                string data = await _streamReader.ReadLineAsync();
                Console.WriteLine(data);
            } catch (Exception) {
                // ignored
            }
        }

        public void Dispose() {
            _namedPipeClientStream?.Dispose();
        }
    }
}