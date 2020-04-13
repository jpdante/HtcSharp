using HtcSharp.Core;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Logging.Abstractions;

namespace HtcSharp.Server {
    public class Program {
        private HtcServer _htcServer;
        private ManualResetEvent _shutdownEvent;
        private bool _daemonMode = false;

        private static void Main(string[] args) {
            new Program().Start(args).GetAwaiter().GetResult();
        }

        private async Task Start(string[] args) {
            _shutdownEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += Console_CancelKeyPress;
            _daemonMode = false;
            switch (args.Length) {
                case 1 when args[0].Equals("daemon-mode", StringComparison.CurrentCultureIgnoreCase):
                    _htcServer = new HtcServer(Path.Combine(Directory.GetCurrentDirectory(), "HtcConfig.json"));
                    _daemonMode = true;
                    break;
                case 1:
                    _htcServer = new HtcServer(args[0]);
                    break;
                case 2 when args[0].Equals("daemon-mode", StringComparison.CurrentCultureIgnoreCase):
                    _htcServer = new HtcServer(args[1]);
                    _daemonMode = true;
                    break;
                case 2 when args[1].Equals("daemon-mode", StringComparison.CurrentCultureIgnoreCase):
                    _htcServer = new HtcServer(args[0]);
                    _daemonMode = true;
                    break;
                case 2:
                    _htcServer = new HtcServer(args[0]);
                    break;
                default:
                    _htcServer = new HtcServer(Path.Combine(Directory.GetCurrentDirectory(), "HtcConfig.json"));
                    break;
            }

            _htcServer.Logger.AddLogger("Console", new ConsoleLogger());
            _htcServer.Logger.AddLogger("File", new FileLogger(Path.Combine(Directory.GetCurrentDirectory(), "HtcSharp.log")));
            await _htcServer.Start();
            if (!_daemonMode) {
                _ = Task.Run(async () => {
                    _shutdownEvent.WaitOne();
                    await _htcServer.Stop();
                    Environment.Exit(0);
                });
                while (_htcServer.Running) {
                    string line = Console.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    switch (line.ToLower()) {
                        case "exit":
                        case "stop":
                            await _htcServer.Stop();
                            break;
                    }
                }
            } else {
                _shutdownEvent.WaitOne();
                await _htcServer.Stop();
            }
        }

        private async void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            _htcServer.Logger.LogInfo("Exiting system due to external CTRL-C, or process kill, or shutdown.", null);
            _shutdownEvent.Set();
            e.Cancel = true;
        }
    }
}