// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace HtcSharp.HttpModule.Core.Internal.Infrastructure {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Infrastructure\DebuggerWrapper.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    internal sealed class DebuggerWrapper : IDebugger {
        private DebuggerWrapper() {
        }

        public static IDebugger Singleton { get; } = new DebuggerWrapper();

        public bool IsAttached => Debugger.IsAttached;
    }
}