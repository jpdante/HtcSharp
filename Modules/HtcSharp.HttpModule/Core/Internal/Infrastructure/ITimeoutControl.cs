// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using HtcSharp.HttpModule.Core.Internal.Http2.FlowControl;

namespace HtcSharp.HttpModule.Core.Internal.Infrastructure {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Infrastructure\ITimeoutControl.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    internal interface ITimeoutControl {
        TimeoutReason TimerReason { get; }

        void SetTimeout(long ticks, TimeoutReason timeoutReason);
        void ResetTimeout(long ticks, TimeoutReason timeoutReason);
        void CancelTimeout();

        void InitializeHttp2(InputFlowControl connectionInputFlowControl);
        void StartRequestBody(MinDataRate minRate);
        void StopRequestBody();
        void StartTimingRead();
        void StopTimingRead();
        void BytesRead(long count);

        void StartTimingWrite();
        void StopTimingWrite();
        void BytesWrittenToBuffer(MinDataRate minRate, long count);
    }
}