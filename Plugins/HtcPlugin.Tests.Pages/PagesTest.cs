using System;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.Abstractions;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Http;

namespace HtcPlugin.Tests.Pages {
    public class PagesTest : IPlugin, IHttpPage {

        internal static readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public string Name => "PagesTest";
        public string Version => "1.0.0";

        public Task Init(IServiceProvider serviceProvider) {
            Logger.LogInfo("Loading...");
            this.RegisterPage("/pages/test", this);
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

        public async Task OnHttpPageRequest(DirectiveDelegate next, HtcHttpContext httpContext, string fileName) {
            await httpContext.Response.WriteAsync($"Requested page: {fileName}");
        }

        public void Dispose() {
        }
    }
}
