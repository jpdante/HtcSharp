using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.IO;
using HtcSharp.HttpModule.Routing.Abstractions;
using HtcSharp.HttpModule.Routing.Error;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class ReturnDirective : IDirective {

        private readonly int _statusCode;
        private readonly string _data;
        private readonly byte _type;

        public ReturnDirective(IReadOnlyList<string> returnData) {
            if (returnData.Count == 2) {
                if (int.TryParse(returnData[1], out var statusCode)) {
                    _statusCode = statusCode;
                    _type = 1;
                } else {
                    _statusCode = 301;
                    _data = returnData[1];
                    _type = 2;
                }
            } else if (returnData.Count >= 3) {
                if (!int.TryParse(returnData[1], out var statusCode)) return;
                _statusCode = statusCode;
                if (returnData[2][0].Equals('"') && returnData[returnData.Count - 1][returnData[returnData.Count - 1].Length - 1].Equals('"')) {
                    _type = 3;
                    for (var i = 2; i < returnData.Count; i++) { _data = i == returnData.Count - 1 ? $"{returnData[i]}" : $"{returnData[i]} "; }
                } else {
                    _type = 2;
                    for (var i = 2; i < returnData.Count; i++) { _data = i == returnData.Count - 1 ? $"{returnData[i]}" : $"{returnData[i]} "; }
                }
            }
        }

        public async Task Execute(HttpContext context) {
            if (_type == 1) {
                await ErrorMessageManager.SendError(context, _statusCode);
            } else if (_type == 2) {
                context.Response.StatusCode = _statusCode;
                context.Response.Headers.Add("Location", HttpIO.ReplaceVars(context, _data));
            } else if (_type == 3) {
                context.Response.StatusCode = _statusCode;
                await context.Response.WriteAsync(HttpIO.ReplaceVars(context, _data));
            }
        }
    }
}