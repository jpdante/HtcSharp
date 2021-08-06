namespace HtcSharp.Abstractions.Manager {
    public interface IReadOnlyPlugin {

        string Name { get; }
        string Version { get; }

        bool IsCompatible(IVersion version);

    }
}