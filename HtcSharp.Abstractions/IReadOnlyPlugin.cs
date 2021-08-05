namespace HtcSharp.Abstractions {
    public interface IReadOnlyPlugin {

        string Name { get; }
        string Version { get; }

        bool IsCompatible(IVersion version);

    }
}