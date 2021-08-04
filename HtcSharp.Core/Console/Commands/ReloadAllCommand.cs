﻿using System.IO;
using System.Threading.Tasks;
using HtcSharp.Abstractions.Internal;
using HtcSharp.Abstractions.Internal.Console;

namespace HtcSharp.Core.Console.Commands {
    public class ReloadAllCommand : CliCommand {

        public override string Command => "reload-all";

        public ReloadAllCommand(IServer server) : base(server) {
        }

        public override async Task Process(StreamReader reader, StreamWriter writer, string data) {
            await writer.WriteLineAsync("Reloading plugins and modules...");
            await Context.OnReload();
        }
    }
}