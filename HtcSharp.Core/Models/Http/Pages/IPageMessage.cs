using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core.Models.Http.Pages {
    public interface IPageMessage {
        int StatusCode { get; }
        string GetPageMessage(HtcHttpContext httpContext);
        void ExecutePageMessage(HtcHttpContext httpContext);
    }
}
