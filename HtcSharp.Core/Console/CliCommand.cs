using System.IO;
using System.Threading.Tasks;
using HtcSharp.Abstractions;

namespace HtcSharp.Core.Console {
    internal abstract class CliCommand {

        public abstract string Command { get; }

        protected IServer Context { get; }

        protected CliCommand(IServer server) {
            Context = server;
        }

        public abstract Task Process(StreamReader reader, StreamWriter writer, string data);

    }
}