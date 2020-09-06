// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Core.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Features\IDecrementConcurrentConnectionCountFeature.cs
    // Start-At-Remote-Line 5
    // SourceTools-End
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