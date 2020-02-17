// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using HtcSharp.HttpModule.Connections.Abstractions;

namespace HtcSharp.HttpModule.Http.Abstractions.Features {
    public interface ITransferFormatFeature {
        TransferFormat SupportedFormats { get; }
        TransferFormat ActiveFormat { get; set; }
    }
}