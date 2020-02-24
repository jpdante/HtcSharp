using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;

namespace HtcSharp.Core.Plugin.Abstractions {
    public interface IPlugin {
        string Name { get; }
        string Version { get; }
        Task Load(PluginServerContext pluginServerContext, ILogger logger);
        Task Enable();
        Task Disable();
        bool IsCompatible(int htcMajor, int htcMinor, int htcPatch);
    }
}