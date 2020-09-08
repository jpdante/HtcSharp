using System;
using HtcSharp.HttpModule.Http.Abstractions;
using Microsoft.Extensions.FileProviders;

namespace HtcSharp.HttpModule.Routing {
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
