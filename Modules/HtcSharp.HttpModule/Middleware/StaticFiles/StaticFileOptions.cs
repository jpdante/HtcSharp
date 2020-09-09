// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using HtcSharp.HttpModule.Http.Features;
using HtcSharp.HttpModule.Middleware.StaticFiles.Infrastructure;

namespace HtcSharp.HttpModule.Middleware.StaticFiles {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Middleware\StaticFiles\src\StaticFileOptions.cs
    // Start-At-Remote-Line 10
    // Ignore-Local-Line-Range 28-52
    // SourceTools-End
    /// <summary>
    /// Options for serving static files
    /// </summary>
    public class StaticFileOptions : SharedOptionsBase {
        /// <summary>
        /// Defaults to all request paths
        /// </summary>
        public StaticFileOptions() : this(new SharedOptions()) {
        }

        /// <summary>
        /// Defaults to all request paths
        /// </summary>
        /// <param name="sharedOptions"></param>
        public StaticFileOptions(SharedOptions sharedOptions) : base(sharedOptions) { }

        /// <summary>
        /// Used to map files to content-types.
        /// </summary>
        public IContentTypeProvider ContentTypeProvider { get; set; }

        /// <summary>
        /// The default content type for a request if the ContentTypeProvider cannot determine one.
        /// None is provided by default, so the client must determine the format themselves.
        /// http://www.w3.org/Protocols/rfc2616/rfc2616-sec7.html#sec7
        /// </summary>
        public string DefaultContentType { get; set; }

        /// <summary>
        /// Indicates if files should be compressed for HTTPS requests when the Response Compression middleware is available.
        /// The default value is <see cref="HttpsCompressionMode.Compress"/>.
        /// </summary>
        /// <remarks>
        /// Enabling compression on HTTPS requests for remotely manipulable content may expose security problems.
        /// </remarks>
        public HttpsCompressionMode HttpsCompression { get; set; } = HttpsCompressionMode.Compress;

    }
}