// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule.Middleware.StaticFiles {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Middleware\StaticFiles\src\StaticFileContext.cs
    // Start-At-Remote-Line 9
    // Ignore-Local-Line-Range 14-51
    // SourceTools-End
    /// <summary>
    /// Contains information about the request and the file that will be served in response.
    /// </summary>
    public class StaticFileResponseContext {
        /// <summary>
        /// Constructs the <see cref="StaticFileResponseContext"/>.
        /// </summary>
        [Obsolete("Use the constructor that passes in the HttpContext and IFileInfo parameters: StaticFileResponseContext(HttpContext context, IFileInfo file)", false)]
        public StaticFileResponseContext() {
        }

        /// <summary>
        /// Constructs the <see cref="StaticFileResponseContext"/>.
        /// </summary>
        /// <param name="context">The request and response information.</param>
        /// <param name="file">The file to be served.</param>
        public StaticFileResponseContext(HttpContext context, FileInfo file) {
            if (file == null) {
                throw new ArgumentNullException(nameof(file));
            }
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }
            Context = context;
            File = file;
        }

        /// <summary>
        /// The request and response information.
        /// </summary>
        public HttpContext Context { get; }

        /// <summary>
        /// The file to be served.
        /// </summary>
        public FileInfo File { get; }
    }
}
