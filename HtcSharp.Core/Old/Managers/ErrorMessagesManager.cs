using System.Collections.Generic;
using HtcSharp.Core.Old.Models.Http;
using HtcSharp.Core.Old.Models.Http.Pages;

namespace HtcSharp.Core.Old.Managers {
    public class ErrorMessagesManager {
        private static readonly Dictionary<int, IPageMessage> DefaultPages;
        private readonly Dictionary<int, IPageMessage> _overridePages;

        static ErrorMessagesManager() {
            DefaultPages = new Dictionary<int, IPageMessage>();
        }

        public ErrorMessagesManager() {
            _overridePages = new Dictionary<int, IPageMessage>();
        }

        public static void RegisterDefaultPage(IPageMessage pageMessage) => DefaultPages[pageMessage.StatusCode] = pageMessage;

        public static void UnRegisterDefaultPage(int statusCode) => DefaultPages.Remove(statusCode);

        public static void ClearDefaultPages() => DefaultPages.Clear();

        public void RegisterOverridePage(IPageMessage pageMessage) => _overridePages[pageMessage.StatusCode] = pageMessage;

        public void UnRegisterOverridePage(int statusCode) => _overridePages.Remove(statusCode);

        public string GetErrorMessage(HtcHttpContext httpContext, int statusCode) {
            if (_overridePages.ContainsKey(statusCode)) return _overridePages[statusCode].GetPageMessage(httpContext);
            else if (DefaultPages.ContainsKey(statusCode)) return DefaultPages[statusCode].GetPageMessage(httpContext);
            else return null;
        }

        public string GetDefaultErrorMessage(HtcHttpContext httpContext, int statusCode) {
            return DefaultPages.ContainsKey(statusCode) ? DefaultPages[statusCode].GetPageMessage(httpContext) : null;
        }

        public void SendError(HtcHttpContext httpContext, int statusCode) {
            if (_overridePages.ContainsKey(statusCode)) _overridePages[statusCode].ExecutePageMessage(httpContext);
            else if (DefaultPages.ContainsKey(statusCode)) DefaultPages[statusCode].ExecutePageMessage(httpContext);
            else httpContext.Response.StatusCode = statusCode;
        }

        public void SendDefaultError(HtcHttpContext httpContext, int statusCode) {
            if (DefaultPages.ContainsKey(statusCode)) DefaultPages[statusCode].ExecutePageMessage(httpContext);
            else httpContext.Response.StatusCode = statusCode;
        }
    }
}
