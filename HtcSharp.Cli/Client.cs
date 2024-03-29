﻿using System;
using System.Threading.Tasks;
using HtcSharp.Cli.Core;
using HtcSharp.Cli.Internal;

namespace HtcSharp.Cli {
    public class Client : ConsoleApplication {

        private ArgsReader ArgsReader;
        private CliClient CliClient;

        protected override Task OnLoad() {
            ArgsReader = new ArgsReader(Args);
            CliClient = new CliClient(ArgsReader.GetOrDefault("server", ".\\"), ArgsReader.GetOrDefault("pipe", "htcsharp"));
            CliClient.Disconnect += CliClientOnDisconnect;
            return Task.CompletedTask;
        }

        private void CliClientOnDisconnect() {
            Environment.Exit(0);
        }

        protected override Task OnStart() {
            Task.Run(Connect);
            return Task.CompletedTask;
        }

        protected override async Task OnExit() {
            await CliClient.Stop();
        }

        private async Task ProcessCommand(string command) {
            await CliClient.SendCommand(command);
        }

        private async Task Connect() {
            await CliClient.Start();
            if (ArgsReader.UnknownArgs.Length > 0) {
                await ProcessCommand(ArgsReader.UnknownArgs);
            } else {
                await ProcessCommands();
            }
        }

        private async Task ProcessCommands() {
            Console.WriteLine("Using CLI mode, press Ctrl + C to exit.");
            await CliClient.SendCommand("cli-mode\n");
            while (Running) {
                string command = Console.ReadLine();
                if (string.IsNullOrEmpty(command)) continue;
                await CliClient.SendCommand(command);
            }
        }
    }
}