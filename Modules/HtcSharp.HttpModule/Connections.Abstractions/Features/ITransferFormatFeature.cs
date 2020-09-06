// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace HtcSharp.HttpModule.Connections.Abstractions.Features {
    public interface ITransferFormatFeature {
        TransferFormat SupportedFormats { get; }
        TransferFormat ActiveFormat { get; set; }
    }
}