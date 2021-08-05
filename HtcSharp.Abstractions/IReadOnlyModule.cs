namespace HtcSharp.Abstractions {
    public interface IReadOnlyModule {
        
        string Name { get; }
        string Version { get; }

        bool IsCompatible(IVersion version);

    }
}