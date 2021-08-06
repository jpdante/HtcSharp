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

        private readonly NamedPipeClientStream _namedPipeClientStream;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;
        private Task _readerTask;

        public delegate void DisconnectHandler();
        public event DisconnectHandler Disconnect;

        public CliClient(string serverName, string pipeName) {
            ServerName = serverName;
            PipeName = pipeName;
            _cancellationTokenSource = new CancellationTokenSource();
            _namedPipeClientStream = new NamedPipeClientStream(ServerName, PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        }

        public async Task Start() {
            await _namedPipeClientStream.ConnectAsync(_cancellationTokenSource.Token);
            _streamReader = new StreamReader(_namedPipeClientStream, Encoding.UTF8);
            _streamWriter = new StreamWriter(_namedPipeClientStream, Encoding.UTF8);
            _readerTask = Task.Run(Reader, _cancellationTokenSource.Token);
        }

        public async Task Stop() {
            _cancellationTokenSource.Cancel();
            await _namedPipeClientStream.DisposeAsync();
            if (_readerTask != null) await _readerTask;
        }

        public async Task SendCommand(string command) {
            await _streamWriter.WriteLineAsync(command);
            await _streamWriter.FlushAsync();
        }

        public async Task Reader() {
            while (_namedPipeClientStream.IsConnected && !_cancellationTokenSource.IsCancellationRequested) {
                try {
                    var buffer = new char[1024];
                    int count = await _streamReader.ReadAsync(buffer, 0, buffer.Length);
                    Console.Write(buffer, 0, count);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            Disconnect?.Invoke();
        }

        public void Dispose() {
            _namedPipeClientStream?.Dispose();
        }
    }
}