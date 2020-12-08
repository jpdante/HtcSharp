using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Extensions;
using HtcSharp.HttpModule.Routing.Abstractions;

namespace HtcSharp.HttpModule.Routing.Directives {
    public class ReturnDirective : IDirective {
        // SourceTools-Start
        // Ignore-Copyright
        // SourceTools-End
        private readonly int _statusCode;
        private readonly string _data;
        private readonly int _type;

        public ReturnDirective(string rawReturn) {
            string[] returnData = rawReturn.Split(" ");
            if (returnData.Length == 1) {
                if (int.TryParse(returnData[0], out int statusCode)) {
                    _statusCode = statusCode;
                    _type = 1;
                } else {
                    _statusCode = 301;
                    _data = returnData[0];
                    _type = 2;
                }
            } else if (returnData.Length >= 2) {
                if (!int.TryParse(returnData[0], out int statusCode)) return;
                _statusCode = statusCode;
                if (returnData[1][0].Equals('"') && returnData[^1][returnData[^1].Length - 1].Equals('"')) {
                    _type = 3;
                    for (var i = 2; i < returnData.Length; i++) {
                        _data = i == returnData.Length - 1 ? $"{returnData[i]}" : $"{returnData[i]} ";
                    }
                } else {
                    _type = 2;
                    for (var i = 2; i < returnData.Length; i++) {
                        _data = i == returnData.Length - 1 ? $"{returnData[i]}" : $"{returnData[i]} ";
                    }
                }
            }
        }

        public async Task Execute(HttpContext context) {
            switch (_type) {
                case 1:
                    await context.ServerInfo.ErrorMessageManager.SendError(context, _statusCode);
                    context.Response.HasFinished = true;
                    break;
                case 2:
                    context.Response.StatusCode = _statusCode;
                    context.Response.Headers.Add("Location", _data.ReplaceHttpContextVars(context));
                    context.Response.HasFinished = true;
                    break;
                case 3:
                    context.Response.StatusCode = _statusCode;
                    await context.Response.WriteAsync(_data.ReplaceHttpContextVars(context));
                    context.Response.HasFinished = true;
                    break;
            }
        }
    }
}