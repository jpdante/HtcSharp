// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using HtcSharp.HttpModule.Http.Features;

namespace HtcSharp.HttpModule.Http.Abstractions {
    public interface IHttpContextFactory {
        HttpContext Create(IFeatureCollection featureCollection);
        void Dispose(HttpContext httpContext);
    }
}