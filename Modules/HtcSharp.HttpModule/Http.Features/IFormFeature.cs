// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Http.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http.Features\src\IFormFeature.cs
    // Start-At-Remote-Line 8
    // SourceTools-End
    public interface IFormFeature {
        /// <summary>
        /// Indicates if the request has a supported form content-type.
        /// </summary>
        bool HasFormContentType { get; }

        /// <summary>
        /// The parsed form, if any.
        /// </summary>
        IFormCollection Form { get; set; }

        /// <summary>
        /// Parses the request body as a form.
        /// </summary>
        /// <returns></returns>
        IFormCollection ReadForm();

        /// <summary>
        /// Parses the request body as a form.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken);
    }
}