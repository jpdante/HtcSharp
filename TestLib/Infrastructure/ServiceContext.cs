// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO.Pipelines;
using TestLib.Http.Protocols.Http;
using TestLib.Infrastructure.Heart;

namespace TestLib.Infrastructure {
    internal class ServiceContext {
        public IKestrelTrace Log { get; set; }

        public PipeScheduler Scheduler { get; set; }

        public IHttpParser<Http1ParsingHandler> HttpParser { get; set; }

        public ISystemClock SystemClock { get; set; }

        public DateHeaderValueManager DateHeaderValueManager { get; set; }

        public ConnectionManager ConnectionManager { get; set; }

        public Heartbeat Heartbeat { get; set; }

        public KestrelServerOptions ServerOptions { get; set; }
    }
}