using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HtcSharp.HttpModule {
    public static class HttpExtensions {

        public static Task WriteJsonAsync(this HttpResponse httpResponse, object obj, JsonSerializerOptions serializerOptions = default, CancellationToken cancellationToken = default) {
            return JsonSerializer.SerializeAsync(httpResponse.Body, obj, serializerOptions, cancellationToken);
        }

        public static Task WriteJsonAsync(this HttpResponse httpResponse, object obj, Type type, JsonSerializerOptions serializerOptions = default, CancellationToken cancellationToken = default) {
            return JsonSerializer.SerializeAsync(httpResponse.Body, obj, type, serializerOptions, cancellationToken);
        }

    }
}