namespace HtcSharp.HttpModule.Infrastructure.Features {
    /// <summary>
    /// A connection feature allowing middleware to stop counting connections towards <see cref="KestrelServerLimits.MaxConcurrentConnections"/>.
    /// This is used by Kestrel internally to stop counting upgraded connections towards this limit.
    /// </summary>
    public interface IDecrementConcurrentConnectionCountFeature {
        /// <summary>
        /// Idempotent method to stop counting a connection towards <see cref="KestrelServerLimits.MaxConcurrentConnections"/>.
        /// </summary>
        void ReleaseConnection();
    }
}
