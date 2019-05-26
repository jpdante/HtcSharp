using HtcSharp.Core.Models.Http.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HtcSharp.Core.Models.Http {
    public class HttpResponseContext {

        private readonly HttpResponse _response;

        public Stream OutputStream => _response.Body;
        public long? ContentLength { get => _response.ContentLength;
            set => _response.ContentLength = value;
        }
        public string ContentType { get => _response.ContentType;
            set => _response.ContentType = value;
        }
        public int StatusCode { get => _response.StatusCode;
            set => _response.StatusCode = value;
        }
        public bool HasStarted => _response.HasStarted;
        public HtcResponseHeaders Headers { get; }
        public HtcResponseCookies Cookies { get; }

        public HttpResponseContext(HttpResponse response) {
            _response = response;
            Headers = new HtcResponseHeaders(response.Headers);
            Cookies = new HtcResponseCookies(response.Cookies);
        }

        public async Task WriteAsync(string text) => await _response.WriteAsync(text);
        public void Write(string text) => _response.Body.Write(Encoding.UTF8.GetBytes(text));

        // Not used
        // public async Task WriteAsync(string text, CancellationToken cancellationToken = default(CancellationToken)) => await _response.WriteAsync(text, cancellationToken);
        public void Redirect(string location) {
            _response.Redirect(location);
        }
    }
}
