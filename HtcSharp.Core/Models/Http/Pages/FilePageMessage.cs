using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HtcSharp.Core.Models.Http.Pages {
    public class FilePageMessage : IPageMessage {

        private readonly string _pageFileName;   
        public int StatusCode { get; }

        public FilePageMessage(string fileName, int statusCode) {
            _pageFileName = fileName;
            StatusCode = statusCode;
        }

        public string GetPageMessage(HtcHttpContext httpContext) {
            var fileContent = File.ReadAllText(_pageFileName, Encoding.UTF8);
            fileContent = fileContent.Replace("%REQUESTURL%", httpContext.Request.Path);
            return fileContent;
        }

        public void ExecutePageMessage(HtcHttpContext httpContext) {
            if (httpContext.Response.HasStarted) return;
            var fileContent = File.ReadAllText(_pageFileName, Encoding.UTF8);
            fileContent = fileContent.Replace("%REQUESTURL%", httpContext.Request.Path);
            httpContext.Response.WriteAsync(fileContent).GetAwaiter().GetResult();
        }
    }
}
