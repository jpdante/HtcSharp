﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace TestLib.Infrastructure {
    internal enum TimeoutReason {
        None,
        KeepAlive,
        RequestHeaders,
        ReadDataRate,
        WriteDataRate,
        RequestBodyDrain,
        TimeoutFeature,
    }
}