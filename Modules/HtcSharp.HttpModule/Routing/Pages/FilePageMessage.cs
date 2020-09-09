using System.IO;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Extensions;
using HtcSharp.HttpModule.Routing.Abstractions;

namespace HtcSharp.HttpModule.Routing.Pages {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    public class FilePageMessage : IPageMessage {
        private readonly string _pageFileName;
        public int StatusCode { get; }

        public FilePageMessage(string fileName, int statusCode) {
            _pageFileName = fileName;
            StatusCode = statusCode;
        }

        public string GetPageMessage(HttpContext httpContext) {
            var fileContent = File.ReadAllText(_pageFileName, Encoding.UTF8);
            fileContent = fileContent.Replace("{Request.Path}", httpContext.Request.Path);
            fileContent = fileContent.Replace("{Request.Host}", httpContext.Request.Host.ToString());
            fileContent = fileContent.Replace("{Request.PathBase}", httpContext.Request.PathBase);
            fileContent = fileContent.Replace("{Request.Protocol}", httpContext.Request.Protocol);
            fileContent = fileContent.Replace("{Request.QueryString}", httpContext.Request.QueryString.ToString());
            fileContent = fileContent.Replace("{Request.RequestFilePath}", httpContext.Request.RequestFilePath);
            fileContent = fileContent.Replace("{Request.RequestPath}", httpContext.Request.RequestPath);
            fileContent = fileContent.Replace("{Request.Scheme}", httpContext.Request.Scheme);
            fileContent = fileContent.Replace("{Request.TranslatedPath}", httpContext.Request.TranslatedPath);
            fileContent = fileContent.Replace("{Request.IsHttps}", httpContext.Request.IsHttps.ToString());
            fileContent = fileContent.Replace("{Request.Method}", httpContext.Request.Method.ToString());
            fileContent = fileContent.Replace("{Connection.Id}", httpContext.Connection.Id);
            fileContent = fileContent.Replace("{Connection.RemoteIpAddress}", httpContext.Connection.RemoteIpAddress.ToString());
            fileContent = fileContent.Replace("{Connection.RemotePort}", httpContext.Connection.RemotePort.ToString());
            return fileContent;
        }

        public async Task ExecutePageMessage(HttpContext httpContext) {
            if (httpContext.Response.HasStarted) return;
            var fileContent = File.ReadAllText(_pageFileName, Encoding.UTF8);
            fileContent = fileContent.Replace("{Request.Path}", httpContext.Request.Path);
            fileContent = fileContent.Replace("{Request.Host}", httpContext.Request.Host.ToString());
            fileContent = fileContent.Replace("{Request.PathBase}", httpContext.Request.PathBase);
            fileContent = fileContent.Replace("{Request.Protocol}", httpContext.Request.Protocol);
            fileContent = fileContent.Replace("{Request.QueryString}", httpContext.Request.QueryString.ToString());
            fileContent = fileContent.Replace("{Request.RequestFilePath}", httpContext.Request.RequestFilePath);
            fileContent = fileContent.Replace("{Request.RequestPath}", httpContext.Request.RequestPath);
            fileContent = fileContent.Replace("{Request.Scheme}", httpContext.Request.Scheme);
            fileContent = fileContent.Replace("{Request.TranslatedPath}", httpContext.Request.TranslatedPath);
            fileContent = fileContent.Replace("{Request.IsHttps}", httpContext.Request.IsHttps.ToString());
            fileContent = fileContent.Replace("{Request.Method}", httpContext.Request.Method.ToString());
            fileContent = fileContent.Replace("{Connection.Id}", httpContext.Connection.Id);
            fileContent = fileContent.Replace("{Connection.RemoteIpAddress}", httpContext.Connection.RemoteIpAddress.ToString());
            fileContent = fileContent.Replace("{Connection.RemotePort}", httpContext.Connection.RemotePort.ToString());
            httpContext.Response.StatusCode = StatusCode;
            await httpContext.Response.WriteAsync(fileContent);
        }
    }
}