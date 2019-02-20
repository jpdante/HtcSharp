using HTCSharp.Core;
using HTCSharp.Core.Logging;
using HTCSharp.Core.Logging.Loggers;
using System;
using System.IO;
using System.Reflection;

namespace HTCSharp.Server {
    public class Program {
        private static readonly ILog _Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);
        private HTCServer htcServer;

        static void Main(string[] args) {
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
                    htcServer = new HTCServer(Path.Combine(Directory.GetCurrentDirectory(), "HTCConfig.json"));
                } else htcServer = new HTCServer(args[0]);
            } else if (args.Length == 2) {
                if (args[0].Equals("daemode", StringComparison.CurrentCultureIgnoreCase)) {
                    daemon = true;
                    htcServer = new HTCServer(args[1]);
                } else if (args[1].Equals("daemode", StringComparison.CurrentCultureIgnoreCase)) {
                    daemon = true;
                    htcServer = new HTCServer(args[0]);
                } else {
                    htcServer = new HTCServer(args[0]);
                }
            } else {
                htcServer = new HTCServer(Path.Combine(Directory.GetCurrentDirectory(), "HTCConfig.json"));
            }
            htcServer.Start();
            htcServer.WaitStop(daemon);
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            _Logger.Info("Exiting system due to external CTRL-C, or process kill, or shutdown.");
            htcServer.Stop();
            Environment.Exit(0);
        }
    }
}
