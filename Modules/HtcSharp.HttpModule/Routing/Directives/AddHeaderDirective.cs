using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing.Abstractions;

namespace HtcSharp.HttpModule.Routing.Directives {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    public class AddHeaderDirective : IDirective {
        private readonly string _name;
        private readonly string _value;

        public AddHeaderDirective(string header) {
            string[] headerData = header.Split(" ", 2);
            if (headerData.Length != 2) return;
            _name = headerData[0];
            _value = headerData[1];
        }

#pragma warning disable 1998
        public Task Execute(HttpContext context) {
            context.Response.StatusCode = 200;
            context.Response.Headers[_name] = _value;
            return Task.CompletedTask;
        }
    }
#pragma warning restore 1998
}