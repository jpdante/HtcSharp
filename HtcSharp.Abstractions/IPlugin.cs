using System.Threading.Tasks;

namespace HtcSharp.Abstractions {
    public interface IPlugin {
        
        string Name { get; }
        string Version { get; }

        Task Load();
        Task Enable();
        Task Disable();

        bool IsCompatible(int htcMajor, int htcMinor, int htcPatch);
    }
}