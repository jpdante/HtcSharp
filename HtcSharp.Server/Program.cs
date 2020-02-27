using HtcSharp.Core;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Logging.Abstractions;

namespace HtcSharp.Server {
    public class Program {
        internal HtcServer HtcServer;

        private static void Main(string[] args) {
            new Program().Start(args).GetAwaiter().GetResult();
        }

        public async Task Start(string[] args) {
            Console.CancelKeyPress += Console_CancelKeyPress;
            switch (args.Length) {
                case 1 when args[0].Equals("daemon-mode", StringComparison.CurrentCultureIgnoreCase):
                    HtcServer = new HtcServer(Path.Combine(Directory.GetCurrentDirectory(), "HtcConfig.json"));
                    break;
                case 1:
                    HtcServer = new HtcServer(args[0]);
                    break;
                case 2 when args[0].Equals("daemon-mode", StringComparison.CurrentCultureIgnoreCase):
                    HtcServer = new HtcServer(args[1]);
                    break;
                case 2 when args[1].Equals("daemon-mode", StringComparison.CurrentCultureIgnoreCase):
                    HtcServer = new HtcServer(args[0]);
                    break;
                case 2:
                    HtcServer = new HtcServer(args[0]);
                    break;
                default:
                    HtcServer = new HtcServer(Path.Combine(Directory.GetCurrentDirectory(), "HtcConfig.json"));
                    break;
            }
            HtcServer.Logger.AddLogger("Console", new ConsoleLogger());
            HtcServer.Logger.AddLogger("File", new FileLogger(Path.Combine(Directory.GetCurrentDirectory(), "HtcSharp.log")));
            await HtcServer.Start();
            while (HtcServer.Running) {
                string line = Console.ReadLine();
                if(string.IsNullOrEmpty(line)) continue;
                switch (line.ToLower()) {
                    case "exit":
                    case "stop":
                        await HtcServer.Stop();
                        break;
                }
            }
        }

        private async void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            HtcServer.Logger.LogInfo("Exiting system due to external CTRL-C, or process kill, or shutdown.", null);
            await HtcServer.Stop();
            Environment.Exit(0);
        }
    }
}
