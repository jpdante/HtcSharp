using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HtcPlugin.Lua.Processor.Core {
    public static class HttpExtensions {

        public static IDictionary<string, string> ToDictionary(this IDictionary<string, StringValues> original) {
            return original.ToDictionary(i => i.Key, i => i.Value.ToString());
        }

        public static IDictionary<string, string> ToDictionary(this IEnumerable<KeyValuePair<string, StringValues>> original) {
            return original.ToDictionary(i => i.Key, i => i.Value.ToString());
        }

        public static IDictionary<string, string> ToDictionary(this IFormCollection original) {
            return original.ToDictionary(i => i.Key, i => i.Value.ToString());
        }

        public static IDictionary<string, string> ToDictionary(this IRequestCookieCollection original) {
            return original.ToDictionary(i => i.Key, i => i.Value.ToString());
        }
    }
}