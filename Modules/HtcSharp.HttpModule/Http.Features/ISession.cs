// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Http.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Features\src\ISession.cs
    // Start-At-Remote-Line 9
    // Ignore-Local-Line-Range 13-72
    // SourceTools-End
    public interface ISession {
        /// <summary>
        /// Indicate whether the current session is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// A unique identifier for the current session.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Custom data for the current session.
        /// </summary>
        object Data { get; }

        /// <summary>
        /// Load the session.
        /// </summary>
        /// <returns></returns>
        Task LoadAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create session.
        /// </summary>
        /// <returns></returns>
        Task CreateAsync(object data = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete session.
        /// </summary>
        /// <returns></returns>
        Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}