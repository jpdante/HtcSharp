using System.Collections.Generic;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing.Abstractions;
using HtcSharp.HttpModule.Routing.Pages;

namespace HtcSharp.HttpModule.Routing.Error {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    public class ErrorMessageManager {
        private static readonly Dictionary<int, IPageMessage> DefaultPages;
        private readonly Dictionary<int, IPageMessage> _overridePages;

        static ErrorMessageManager() {
            DefaultPages = new Dictionary<int, IPageMessage>();
        }

        public ErrorMessageManager() {
            _overridePages = new Dictionary<int, IPageMessage>();
        }

        public static void RegisterDefaultPage(IPageMessage pageMessage) => DefaultPages[pageMessage.StatusCode] = pageMessage;

        public static void UnRegisterDefaultPage(int statusCode) => DefaultPages.Remove(statusCode);

        public static void ClearDefaultPages() => DefaultPages.Clear();

        public void RegisterOverridePage(IPageMessage pageMessage) => _overridePages[pageMessage.StatusCode] = pageMessage;

        public void UnRegisterOverridePage(int statusCode) => _overridePages.Remove(statusCode);

        public async Task<string> GetErrorMessage(HttpContext httpContext, int statusCode) {
            if (_overridePages.ContainsKey(statusCode)) return await _overridePages[statusCode].GetPageMessage(httpContext);
            return DefaultPages.ContainsKey(statusCode) ? await DefaultPages[statusCode].GetPageMessage(httpContext) : null;
        }

        public static async Task<string> GetDefaultErrorMessage(HttpContext httpContext, int statusCode) {
            return DefaultPages.ContainsKey(statusCode) ? await DefaultPages[statusCode].GetPageMessage(httpContext) : null;
        }

        public async Task SendError(HttpContext httpContext, int statusCode) {
            if (_overridePages.ContainsKey(statusCode))
                await _overridePages[statusCode].ExecutePageMessage(httpContext);
            else if (DefaultPages.ContainsKey(statusCode))
                await DefaultPages[statusCode].ExecutePageMessage(httpContext);
            else
                httpContext.Response.StatusCode = statusCode;
        }

        public static async Task SendDefaultError(HttpContext httpContext, int statusCode) {
            if (DefaultPages.ContainsKey(statusCode))
                await DefaultPages[statusCode].ExecutePageMessage(httpContext);
            else
                httpContext.Response.StatusCode = statusCode;
        }
    }
}