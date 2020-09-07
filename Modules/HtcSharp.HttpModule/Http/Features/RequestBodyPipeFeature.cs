// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule.Http.Features {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Http\src\Features\RequestBodyPipeFeature.cs
    // Start-At-Remote-Line 10
    // SourceTools-End
    public class RequestBodyPipeFeature : IRequestBodyPipeFeature {
        private PipeReader _internalPipeReader;
        private Stream _streamInstanceWhenWrapped;
        private HttpContext _context;

        public RequestBodyPipeFeature(HttpContext context) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        public PipeReader Reader {
            get {
                if (_internalPipeReader == null ||
                    !ReferenceEquals(_streamInstanceWhenWrapped, _context.Request.Body)) {
                    _streamInstanceWhenWrapped = _context.Request.Body;
                    _internalPipeReader = PipeReader.Create(_context.Request.Body);

                    _context.Response.OnCompleted((self) => {
                        ((PipeReader)self).Complete();
                        return Task.CompletedTask;
                    }, _internalPipeReader);
                }

                return _internalPipeReader;
            }
        }
    }
}