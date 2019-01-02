using HTCSharp.Core.Models.Http.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HTCSharp.Core.Models.Http {
    public class HttpResponseContext {

        private HttpResponse Response;

        public Stream OutputStream { get { return Response.Body; } }
        public long? ContentLength { get { return Response.ContentLength; } set { Response.ContentLength = value; } }
        public string ContentType { get { return Response.ContentType; } set { Response.ContentType = value; } }
        public int StatusCode { get { return Response.StatusCode; } set { Response.StatusCode = value; } }
        public bool HasStarted { get { return Response.HasStarted; } }
        public HTCResponseHeaders Headers { get; }
        public HTCResponseCookies Cookies { get; }

        public HttpResponseContext(HttpResponse response) {
            Response = response;
            Headers = new HTCResponseHeaders(response.Headers);
            Cookies = new HTCResponseCookies(response.Cookies);
        }

        public async Task WriteAsync(string text) => await Response.WriteAsync(text);
        public async Task WriteAsync(string text, CancellationToken cancellationToken = default(CancellationToken)) => await Response.WriteAsync(text, cancellationToken);
        public void Redirect(string location) {
            Response.Redirect(location);
        }
    }
}
