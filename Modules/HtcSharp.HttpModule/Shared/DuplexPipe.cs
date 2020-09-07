// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// ReSharper disable once CheckNamespace

namespace System.IO.Pipelines {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Servers\Kestrel\shared\DuplexPipe.cs
    // Start-At-Remote-Line 7
    // SourceTools-End
    internal class DuplexPipe : IDuplexPipe {
        public DuplexPipe(PipeReader reader, PipeWriter writer) {
            Input = reader;
            Output = writer;
        }

        public PipeReader Input { get; }

        public PipeWriter Output { get; }

        public static DuplexPipePair CreateConnectionPair(PipeOptions inputOptions, PipeOptions outputOptions) {
            var input = new Pipe(inputOptions);
            var output = new Pipe(outputOptions);

            var transportToApplication = new DuplexPipe(output.Reader, input.Writer);
            var applicationToTransport = new DuplexPipe(input.Reader, output.Writer);

            return new DuplexPipePair(applicationToTransport, transportToApplication);
        }

        // This class exists to work around issues with value tuple on .NET Framework
        public readonly struct DuplexPipePair {
            public IDuplexPipe Transport { get; }
            public IDuplexPipe Application { get; }

            public DuplexPipePair(IDuplexPipe transport, IDuplexPipe application) {
                Transport = transport;
                Application = application;
            }
        }
    }
}