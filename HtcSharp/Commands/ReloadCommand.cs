using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.Abstractions.Cli;
using HtcSharp.Internal;

namespace HtcSharp.Commands {
    internal class ReloadCommand : InternalCliCommand {

        public override string Command => "reload";

        public ReloadCommand(IServer server) : base(server) {
        }

        public override async Task Process(IReader reader, IWriter writer, string data) {
            await writer.WriteLineAsync("Reloading...");
            await Context.OnReload();
            await writer.WriteLineAsync("Reloaded!");
        }
    }
}