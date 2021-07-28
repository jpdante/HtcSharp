using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class AddHeaderDirective : IDirective {

        private readonly DirectiveDelegate _next;
        private readonly Dictionary<string, string> _headers;

        public AddHeaderDirective(DirectiveDelegate next, JsonElement config) {
            _next = next;
            _headers = new Dictionary<string, string>();
            if (!config.TryGetProperty("headers", out var headersProperty)) return;
            foreach (var property in headersProperty.EnumerateObject().Where(property => property.Value.ValueKind == JsonValueKind.String)) {
                _headers.Add(property.Name, property.Value.GetString());
            }
        }

        public Task Invoke(HtcHttpContext httpContext) {
            foreach ((string key, string value) in _headers) {
                httpContext.Response.Headers.Add(key, value);
            }
            return _next(httpContext);
        }

    }
}