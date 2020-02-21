using HtcSharp.Core;
using System;
using System.IO;
using System.Reflection;
using HtcSharp.Core.Old.Logging;
using HtcSharp.Core.Old.Logging.Loggers;

namespace HtcSharp.Server {
    public class Program {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);
        private HtcServer _htcServer;

        private static void Main(string[] args) {
            new Program().Start(args);
        }

        public void Start(string[] args) {
            Console.CancelKeyPress += Console_CancelKeyPress;
            LogManager.RegisterLogger(new ConsoleLogger());
            LogManager.RegisterLogger(new FileLogger(Path.Combine(Directory.GetCurrentDirectory(), "HtcSharp.log")));
            var daemon = false;
            switch (args.Length) {
                case 1 when args[0].Equals("daemon-mode", StringComparison.CurrentCultureIgnoreCase):
                    daemon = true;
                    _htcServer = new HtcServer(Path.Combine(Directory.GetCurrentDirectory(), "HtcConfig.json"));
                    break;
                case 1:
                    _htcServer = new HtcServer(args[0]);
                    break;
                case 2 when args[0].Equals("daemon-mode", StringComparison.CurrentCultureIgnoreCase):
                    daemon = true;
                    _htcServer = new HtcServer(args[1]);
                    break;
                case 2 when args[1].Equals("daemon-mode", StringComparison.CurrentCultureIgnoreCase):
                    daemon = true;
                    _htcServer = new HtcServer(args[0]);
                    break;
                case 2:
                    _htcServer = new HtcServer(args[0]);
                    break;
                default:
                    _htcServer = new HtcServer(Path.Combine(Directory.GetCurrentDirectory(), "HtcConfig.json"));
                    break;
            }
            _htcServer.Start();
            _htcServer.WaitStop(daemon);
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            Logger.Info("Exiting system due to external CTRL-C, or process kill, or shutdown.");
            _htcServer.Stop();
            Environment.Exit(0);
        }
    }
}
