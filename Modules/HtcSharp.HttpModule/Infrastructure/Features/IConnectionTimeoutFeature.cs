using System;

namespace HtcSharp.HttpModule.Infrastructure.Features {
    /// <summary>
    /// Feature for efficiently handling connection timeouts.
    /// </summary>
    public interface IConnectionTimeoutFeature {
        /// <summary>
        /// Close the connection after the specified positive finite <see cref="TimeSpan"/>
        /// unless the timeout is canceled or reset. This will fail if there is an ongoing timeout.
        /// </summary>
        void SetTimeout(TimeSpan timeSpan);

        /// <summary>
        /// Close the connection after the specified positive finite <see cref="TimeSpan"/>
        /// unless the timeout is canceled or reset. This will cancel any ongoing timeouts.
        /// </summary>
        void ResetTimeout(TimeSpan timeSpan);

        /// <summary>
        /// Prevent the connection from closing after a timeout specified by <see cref="SetTimeout(TimeSpan)"/>
        /// or <see cref="ResetTimeout(TimeSpan)"/>.
        /// </summary>
        void CancelTimeout();
    }
}
