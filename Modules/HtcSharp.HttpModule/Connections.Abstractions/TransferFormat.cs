// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace HtcSharp.HttpModule.Connections.Abstractions {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Connections.Abstractions\src\TransferFormat.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    [Flags]
    public enum TransferFormat {
        Binary = 0x01,
        Text = 0x02
    }
}