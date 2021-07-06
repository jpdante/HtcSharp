using System.Threading.Tasks;

namespace HtcSharp.Abstractions {
    public interface IModule {
        string Name { get; }
        string Version { get; }
        Task Load(HtcServer htcServer, ILogger logger);
        Task Enable();
        Task Disable();
        bool IsCompatible(int htcMajor, int htcMinor, int htcPatch);
    }
}