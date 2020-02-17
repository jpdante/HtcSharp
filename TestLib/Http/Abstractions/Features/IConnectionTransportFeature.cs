﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO.Pipelines;

namespace TestLib.Http.Abstractions.Features
{
    public interface IConnectionTransportFeature
    {
        IDuplexPipe Transport { get; set; }
    }
}
