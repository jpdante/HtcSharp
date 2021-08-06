using System.IO;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.Abstractions.Cli;

namespace HtcSharp.Internal {
    internal abstract class InternalCliCommand : ICliCommand {

        public abstract string Command { get; }

        protected IServer Context { get; }

        protected InternalCliCommand(IServer server) {
            Context = server;
        }

        public abstract Task Process(IReader reader, IWriter writer, string data);

    }
}