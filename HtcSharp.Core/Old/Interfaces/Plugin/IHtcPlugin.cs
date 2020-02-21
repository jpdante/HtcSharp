namespace HtcSharp.Core.Old.Interfaces.Plugin {
    public interface IHtcPlugin {
        string PluginName { get; }
        string PluginVersion { get; }
        void OnLoad();
        void OnEnable();
        void OnDisable();
    }
}
