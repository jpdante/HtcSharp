// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace HtcSharp.HttpModule.Http.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Features\src\IHeaderDictionary.cs
    // Start-At-Remote-Line 8
    // SourceTools-End
    /// <summary>
    /// Represents HttpRequest and HttpResponse headers
    /// </summary>
    public interface IHeaderDictionary : IDictionary<string, StringValues> {
        /// <summary>
        /// IHeaderDictionary has a different indexer contract than IDictionary, where it will return StringValues.Empty for missing entries.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The stored value, or StringValues.Empty if the key is not present.</returns>
        new StringValues this[string key] { get; set; }

        /// <summary>
        /// Strongly typed access to the Content-Length header. Implementations must keep this in sync with the string representation.
        /// </summary>
        long? ContentLength { get; set; }
    }
}