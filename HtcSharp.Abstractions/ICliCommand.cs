using System.Threading.Tasks;

namespace HtcSharp.Abstractions {
    public interface ICliCommand {
        
        public string Command { get; }

        public Task<string> Process(string data);

    }
}