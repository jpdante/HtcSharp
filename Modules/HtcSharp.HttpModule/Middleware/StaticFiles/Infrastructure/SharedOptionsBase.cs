// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using HtcSharp.HttpModule.Http.Abstractions;
using Microsoft.Extensions.FileProviders;

namespace HtcSharp.HttpModule.Middleware.StaticFiles.Infrastructure {
    /// <summary>
    /// Options common to several middleware components
    /// </summary>
    public abstract class SharedOptionsBase {
        /// <summary>
        /// Creates an new instance of the SharedOptionsBase.
        /// </summary>
        /// <param name="sharedOptions"></param>
        protected SharedOptionsBase(SharedOptions sharedOptions) {
            if (sharedOptions == null) {
                throw new ArgumentNullException(nameof(sharedOptions));
            }

            SharedOptions = sharedOptions;
        }

        /// <summary>
        /// Options common to several middleware components
        /// </summary>
        protected SharedOptions SharedOptions { get; private set; }
    }
}
