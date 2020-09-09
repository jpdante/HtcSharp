// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using HtcSharp.HttpModule.Http.Abstractions;

namespace HtcSharp.HttpModule.Routing {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    internal static class Helpers {
        internal static bool PathEndsInSlash(PathString path) {
            return path.Value.EndsWith("/", StringComparison.Ordinal);
        }

        internal static bool TryMatchPath(HttpContext context, PathString matchUrl, bool forDirectory, out PathString subpath) {
            var path = context.Request.Path;

            if (forDirectory && !PathEndsInSlash(path)) {
                path += new PathString("/");
            }

            if (path.StartsWithSegments(matchUrl, out subpath)) {
                return true;
            }
            return false;
        }
    }
}
