using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using HtcSharp.HttpModule.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace HtcSharp.HttpModule.Http {
    public class HtcHttpContext : HttpContext {

        private readonly HttpContext _httpContext;

        public HtcHttpContext(HttpContext httpContext, IReadOnlySite site) {
            _httpContext = httpContext;
            Site = site;
        }

        public override IFeatureCollection Features => _httpContext.Features;

        public override HttpRequest Request => _httpContext.Request;

        public override HttpResponse Response => _httpContext.Response;

        public override ConnectionInfo Connection => _httpContext.Connection;

        public override WebSocketManager WebSockets => _httpContext.WebSockets;

        public override ClaimsPrincipal User {
            get => _httpContext.User;
            set => _httpContext.User = value;
        }

        public override IDictionary<object, object> Items {
            get => _httpContext.Items;
            set => _httpContext.Items = value;
        }

        public override IServiceProvider RequestServices {
            get => _httpContext.RequestServices;
            set => _httpContext.RequestServices = value;
        }

        public override CancellationToken RequestAborted {
            get => _httpContext.RequestAborted;
            set => _httpContext.RequestAborted = value;
        }

        public override string TraceIdentifier {
            get => _httpContext.TraceIdentifier;
            set => _httpContext.TraceIdentifier = value;
        }

        public override ISession Session {
            get => _httpContext.Session;
            set => _httpContext.Session = value;
        }

        public IReadOnlySite Site { get; }

        public override void Abort() => _httpContext.Abort();
    }
}