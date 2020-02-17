// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace HtcSharp.HttpModule2.Http.Protocols.Http
{
    [Flags]
    internal enum TransferCoding
    {
        None,
        Chunked,
        Other
    }
}
