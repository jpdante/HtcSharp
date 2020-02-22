using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing.Abstractions;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class AddHeaderDirective : IDirective {

        private readonly string _name;
        private readonly string _value;

        public AddHeaderDirective(IReadOnlyList<string> header) {
            if (header.Count < 3) return;
            _name = header[1];
            _value = header[2];
        }

#pragma warning disable 1998
        public async Task Execute(HttpContext context) {
            context.Response.Headers.Add(_name, _value);
        }
    }
#pragma warning restore 1998
}
