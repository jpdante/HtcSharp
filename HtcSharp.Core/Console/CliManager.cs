using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.Abstractions.Internal.Console;

namespace HtcSharp.Core.Console {
    public class CliManager : IDisposable {

        private readonly Dictionary<string, CliCommand> _commands;

        private NamedPipeServerStream? _namedPipeServerStream;
        private CancellationTokenSource? _cancellationTokenSource;

        public string PipeName { get; private set; }
        public bool Running { get; private set; }
        public bool Connected { get; private set; }
        public IEnumerable<CliCommand> Commands => _commands.Values;

        public CliManager(string pipeName) {
            PipeName = pipeName;
            _commands = new Dictionary<string, CliCommand>();
        }

        public Task Start() {
            Running = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _namedPipeServerStream = new NamedPipeServerStream("htcsharp", PipeDirection.InOut, 1);
            _namedPipeServerStream.BeginWaitForConnection(OnNewConnection, null);
            return Task.CompletedTask;
        }

        public async Task Stop() {
            if (!Running) return;
            Running = false;
            _cancellationTokenSource?.Cancel();
            if (_namedPipeServerStream != null) await _namedPipeServerStream.DisposeAsync()!;
        }

        public void AddCommand(CliCommand command) {
            if (command.Command.Contains(" ")) throw new Exception("Command cannot contain spaces.");
            _commands.Add(command.Command, command);
        }

        public void RemoveCommand(CliCommand command) {
            _commands.Remove(command.Command);
        }

        private async void OnNewConnection(IAsyncResult result) {
            try {
                if (!Running) return;
                if (_namedPipeServerStream == null) throw new NullReferenceException();
                _namedPipeServerStream.EndWaitForConnection(result);
                Connected = true;
                await ProcessCommands();
            } catch (Exception) {
                Connected = false;
            }
        }

        private async Task ProcessCommands() {
            if (_namedPipeServerStream == null) throw new NullReferenceException();
            using var reader = new StreamReader(_namedPipeServerStream, Encoding.UTF8);
            await using var writer = new StreamWriter(_namedPipeServerStream, Encoding.UTF8);

            while (Running) {
                string? line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line)) continue;
                string[] data = line.Split(" ", 2);
                if (_commands.TryGetValue(data[0], out var command)) {
                    await command.Process(reader, writer, data[1]);
                }
            }
        }

        public void Dispose() {
            _namedPipeServerStream?.Dispose();
        }
    }
}