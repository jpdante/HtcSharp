using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Interfaces.Http;
using HtcSharp.Core.Utils.Http;

namespace HtcSharp.Core.Models.Http.Directives {
    public class AddHeaderDirective : IDirective {

        private readonly string _name;
        private readonly string _value;

        public AddHeaderDirective(IReadOnlyList<string> header) {
            if (header.Count < 3) return;
            _name = header[1];
            _value = header[2];
        }

        public void Execute(HtcHttpContext context) {
            context.Response.Headers.Add(_name, _value);
        }
    }
}
