// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Http.Features.Interfaces {
    public interface IRequestCookiesFeature {
        IRequestCookieCollection Cookies { get; set; }
    }
}