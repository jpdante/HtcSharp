using System.Collections.Generic;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing.Pages;

namespace HtcSharp.HttpModule.Routing.Error {
    public static class ErrorMessagesManager {
        private static readonly Dictionary<int, IPageMessage> DefaultPages;
        private static readonly Dictionary<int, IPageMessage> OverridePages;

        static ErrorMessagesManager() {
            DefaultPages = new Dictionary<int, IPageMessage>();
            OverridePages = new Dictionary<int, IPageMessage>();
        }

        public static void RegisterDefaultPage(IPageMessage pageMessage) => DefaultPages[pageMessage.StatusCode] = pageMessage;

        public static void UnRegisterDefaultPage(int statusCode) => DefaultPages.Remove(statusCode);

        public static void ClearDefaultPages() => DefaultPages.Clear();

        public static void RegisterOverridePage(IPageMessage pageMessage) => OverridePages[pageMessage.StatusCode] = pageMessage;

        public static void UnRegisterOverridePage(int statusCode) => OverridePages.Remove(statusCode);

        public static string GetErrorMessage(HttpContext httpContext, int statusCode) {
            if (OverridePages.ContainsKey(statusCode)) return OverridePages[statusCode].GetPageMessage(httpContext);
            else if (DefaultPages.ContainsKey(statusCode)) return DefaultPages[statusCode].GetPageMessage(httpContext);
            else return null;
        }

        public static string GetDefaultErrorMessage(HttpContext httpContext, int statusCode) {
            return DefaultPages.ContainsKey(statusCode) ? DefaultPages[statusCode].GetPageMessage(httpContext) : null;
        }

        public static void SendError(HttpContext httpContext, int statusCode) {
            if (OverridePages.ContainsKey(statusCode)) OverridePages[statusCode].ExecutePageMessage(httpContext);
            else if (DefaultPages.ContainsKey(statusCode)) DefaultPages[statusCode].ExecutePageMessage(httpContext);
            else httpContext.Response.StatusCode = statusCode;
        }

        public static void SendDefaultError(HttpContext httpContext, int statusCode) {
            if (DefaultPages.ContainsKey(statusCode)) DefaultPages[statusCode].ExecutePageMessage(httpContext);
            else httpContext.Response.StatusCode = statusCode;
        }
    }
}
