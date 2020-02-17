// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace TestLib.Http.Abstractions.Features
{
    public interface IConnectionHeartbeatFeature
    {
        void OnHeartbeat(Action<object> action, object state);
    }
}