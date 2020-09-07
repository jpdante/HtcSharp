// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Core.Internal.Http {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\Core\src\Internal\Http\IHttpResponsePipeWriterControl.cs
    // Start-At-Remote-Line 10
    // SourceTools-End
    internal interface IHttpResponsePipeWriterControl {
        void ProduceContinue();
        Memory<byte> GetMemory(int sizeHint = 0);
        Span<byte> GetSpan(int sizeHint = 0);
        void Advance(int bytes);
        ValueTask<FlushResult> FlushPipeAsync(CancellationToken cancellationToken);
        ValueTask<FlushResult> WritePipeAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken);
        void CancelPendingFlush();
    }
}