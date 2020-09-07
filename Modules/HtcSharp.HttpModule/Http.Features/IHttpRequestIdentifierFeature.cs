// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Http.Features {
    /// <summary>
    /// Feature to identify a request.
    /// </summary>
    public interface IHttpRequestIdentifierFeature {
        /// <summary>
        /// Identifier to trace a request.
        /// </summary>
        string TraceIdentifier { get; set; }
    }
}