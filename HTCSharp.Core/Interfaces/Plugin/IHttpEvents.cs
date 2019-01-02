using HTCSharp.Core.Models.Http;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Interfaces.Plugin {
    public interface IHttpEvents {
        bool OnHttpPageRequest(HTCHttpContext context, string filename);
        bool OnHttpExtensionRequest(HTCHttpContext context, string filename, string extension);
    }
}
