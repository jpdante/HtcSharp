// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO.Pipelines;

namespace HtcSharp.HttpModule.Http.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Features\src\IRequestBodyPipeFeature.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    /// <summary>
    /// Represents the HttpRequestBody as a PipeReader.
    /// </summary>
    public interface IRequestBodyPipeFeature {
        /// <summary>
        /// A <see cref="PipeReader"/> representing the request body, if any.
        /// </summary>
        PipeReader Reader { get; }
    }
}