// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using TestLib.Net.Connections.Exceptions;

namespace TestLib.Http.Protocols.Http
{
    internal interface IHttpOutputAborter
    {
        void Abort(ConnectionAbortedException abortReason);
    }
}
