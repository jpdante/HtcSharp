using System.Threading.Tasks;
using HtcSharp.Abstractions.Cli;

namespace HtcSharp.Abstractions {
    public interface ICliCommand {
        
        public string Command { get; }

        public Task Process(IReader reader, IWriter writer, string data);

    }
}