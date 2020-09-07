// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Primitives;

namespace HtcSharp.HttpModule.Http.Headers {
    // SourceTools-Start
    // Remote-File C:\ASP\src\Http\Headers\src\GenericHeaderParser.cs
    // Start-At-Remote-Line 8
    // SourceTools-End
    internal sealed class GenericHeaderParser<T> : BaseHeaderParser<T> {
        internal delegate int GetParsedValueLengthDelegate(StringSegment value, int startIndex, out T parsedValue);

        private GetParsedValueLengthDelegate _getParsedValueLength;

        internal GenericHeaderParser(bool supportsMultipleValues, GetParsedValueLengthDelegate getParsedValueLength)
            : base(supportsMultipleValues) {
            if (getParsedValueLength == null) {
                throw new ArgumentNullException(nameof(getParsedValueLength));
            }

            _getParsedValueLength = getParsedValueLength;
        }

        protected override int GetParsedValueLength(StringSegment value, int startIndex, out T parsedValue) {
            return _getParsedValueLength(value, startIndex, out parsedValue);
        }
    }
}