using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;

namespace HtcSharp.Core.Module.Abstractions {
    public interface IModule {
        string Name { get; }
        string Version { get; }
        Task Load(HtcServer htcServer, ILogger logger);
        Task Enable();
        Task Disable();
        bool IsCompatible(int htcMajor, int htcMinor, int htcPatch);
    }
}