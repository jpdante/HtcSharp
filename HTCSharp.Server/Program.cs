using HTCSharp.Core;
using HTCSharp.Core.Logging;
using HTCSharp.Core.Logging.Loggers;
using System;
using System.IO;
using System.Reflection;

namespace HTCSharp.Server {
    public class Program {
        private static readonly ILog Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);
        private HTCServer _htcServer;

        private static void Main(string[] args) {
            new Program().Start(args);
        }

        public void Start(string[] args) {
            Console.CancelKeyPress += Console_CancelKeyPress;
            LogManager.RegisterLogger(new ConsoleLogger());
            LogManager.RegisterLogger(new FileLogger(Path.Combine(Directory.GetCurrentDirectory(), "HTCSharp.log")));
            bool daemon = false;
            if (args.Length == 1) {
                if (args[0].Equals("daemode", StringComparison.CurrentCultureIgnoreCase)) {
                    daemon = true;
                    _htcServer = new HTCServer(Path.Combine(Directory.GetCurrentDirectory(), "HTCConfig.json"));
                } else _htcServer = new HTCServer(args[0]);
            } else if (args.Length == 2) {
                if (args[0].Equals("daemode", StringComparison.CurrentCultureIgnoreCase)) {
                    daemon = true;
                    _htcServer = new HTCServer(args[1]);
                } else if (args[1].Equals("daemode", StringComparison.CurrentCultureIgnoreCase)) {
                    daemon = true;
                    _htcServer = new HTCServer(args[0]);
                } else {
                    _htcServer = new HTCServer(args[0]);
                }
            } else {
                _htcServer = new HTCServer(Path.Combine(Directory.GetCurrentDirectory(), "HTCConfig.json"));
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
