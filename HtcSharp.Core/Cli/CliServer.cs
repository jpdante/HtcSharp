using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.Core.Internal.Cli;
using HtcSharp.Logging;

namespace HtcSharp.Core.Cli {
    public class CliServer : IDisposable {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly Dictionary<string, ICliCommand> _commands;

        private NamedPipeServerStream? _namedPipeServerStream;
        private CancellationTokenSource? _cancellationTokenSource;

        public string PipeName { get; private set; }
        public bool Running { get; private set; }
        public bool? Connected => _namedPipeServerStream?.IsConnected;
        internal IEnumerable<ICliCommand> Commands => _commands.Values;

        public CliServer(string pipeName) {
            PipeName = pipeName;
            _commands = new Dictionary<string, ICliCommand>();
        }

        public Task Start() {
            Running = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _namedPipeServerStream = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1);
            _namedPipeServerStream.BeginWaitForConnection(OnNewConnection, null);
            return Task.CompletedTask;
        }

        public async Task Stop() {
            if (!Running) return;
            Running = false;
            _cancellationTokenSource?.Cancel();
            if (_namedPipeServerStream != null) await _namedPipeServerStream.DisposeAsync()!;
        }

        public void AddCommand(ICliCommand command) {
            if (command.Command.Contains(" ")) throw new Exception("Command cannot contain spaces.");
            _commands.Add(command.Command, command);
        }

        public void RemoveCommand(ICliCommand command) {
            _commands.Remove(command.Command);
        }

        private async void OnNewConnection(IAsyncResult result) {
            try {
                if (!Running) return;
                if (_namedPipeServerStream == null) throw new NullReferenceException();
                _namedPipeServerStream.EndWaitForConnection(result);
                Logger.LogInfo("New CLI connection");
                await ProcessCommands();
                try {
                    if (_namedPipeServerStream.IsConnected) _namedPipeServerStream.Disconnect();
                } catch {
                    // ignored
                }
                if (Running && _cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested) {
                    _namedPipeServerStream.BeginWaitForConnection(OnNewConnection, null);
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
        }

        private async Task ProcessCommands() {
            try {
                if (_namedPipeServerStream == null) throw new NullReferenceException();
                using var reader = new CliReader(_namedPipeServerStream);
                await using var writer = new CliWriter(_namedPipeServerStream);
                var cliMode = false;
                var firstCmd = true;
                while (_namedPipeServerStream.IsConnected) {
                    string? line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line)) continue;
                    if (firstCmd) {
                        firstCmd = false;
                        if (line == "cli-mode") {
                            cliMode = true;
                            await writer.WriteAsync(">");
                            await writer.FlushAsync();
                            continue;
                        }
                    }
                    if (line == "exit") {
                        await writer.WriteLineAsync("Exiting...");
                        await writer.FlushAsync();
                        return;
                    }
                    Logger.LogInfo($">{line}");
                    string[] data = line.Split(" ", 2);
                    if (_commands.TryGetValue(data[0], out var command)) {
                        await command.Process(reader, writer, data.Length > 1 ? data[1] : null);
                    } else {
                        await writer.WriteLineAsync($"Unknown command '{data[0]}'.");
                    }
                    if (cliMode) await writer.WriteAsync(">");
                    await writer.FlushAsync();
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
        }

        public void Dispose() {
            _namedPipeServerStream?.Dispose();
        }
    }
}