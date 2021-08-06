namespace HtcSharp.Abstractions.Manager {
    public interface IReadOnlyModule {
        
        string Name { get; }
        string Version { get; }

        bool IsCompatible(IVersion version);

    }
}