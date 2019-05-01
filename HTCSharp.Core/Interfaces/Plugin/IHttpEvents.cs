using HtcSharp.Core.Models.Http;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core.Interfaces.Plugin {
    public interface IHttpEvents {
        bool OnHttpPageRequest(HtcHttpContext httpContext, string filename);
        bool OnHttpExtensionRequest(HtcHttpContext httpContext, string filename, string extension);
    }
}
