using System;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Mvc;
using HtcSharp.HttpModule.Mvc.Exceptions;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Http;

namespace HtcPlugin.Tests.Mvc {
    public class MvcTest : HttpMvc, IPlugin {

        internal static readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public string Name => "MvcTest";
        public string Version => "1.0.0";

        public Task Init(IServiceProvider serviceProvider) {
            Logger.LogInfo("Loading...");
            UsePrefix("/mvc");
            MatchDomain("127.0.0.1");
            LoadControllers(GetType().Assembly);
            this.RegisterMvc(this);
            return Task.CompletedTask;
        }

        public Task Enable() {
            Logger.LogInfo("Enabling...");
            return Task.CompletedTask;
        }

        public Task Disable() {
            Logger.LogInfo("Disabling...");
            return Task.CompletedTask;
        }

        public bool IsCompatible(IVersion version) {
            return true;
        }

        protected override async Task ThrowHttpException(HtcHttpContext httpContext, HttpException httpException) {
            if (httpException is HttpDecodeDataException httpDecodeDataException) {
                foreach (var exception in httpDecodeDataException.InnerExceptions) {
                    Logger.LogError(exception);
                }
            }
            httpContext.Response.StatusCode = httpException.Status;
            await httpContext.Response.WriteAsync(httpException.Message);
        }

        public void Dispose() {
        }
    }
}
