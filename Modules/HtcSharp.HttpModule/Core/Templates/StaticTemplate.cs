using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Http;
using Microsoft.AspNetCore.Http;

namespace HtcSharp.HttpModule.Core.Templates {
    public class StaticTemplate : ITemplate {

        public bool SupportGetString => true;
        public string Template { get; }
        public string ContentType { get; }

        public StaticTemplate(string template, string contentType) {
            Template = template;
            ContentType = contentType;
        }

        public StaticTemplate(string template, ContentType contentType) : this(template, contentType.ToValue()) {

        }

        public Task<string> GetString() {
            return Task.FromResult(Template);
        }

        public Task<string> GetReplaced(HtcHttpContext httpContext) {
            var dataBuilder = new StringBuilder(Template);
            dataBuilder.Replace("$Scheme", httpContext.Request.Scheme);
            dataBuilder.Replace("$Path", httpContext.Request.Path.Value);
            dataBuilder.Replace("$ContentType", httpContext.Request.ContentType);
            dataBuilder.Replace("$Method", httpContext.Request.Method);
            dataBuilder.Replace("$Host", httpContext.Request.Host.Value);
            dataBuilder.Replace("$RemoteAddr", httpContext.Connection.RemoteIpAddress.ToString());
            dataBuilder.Replace("$RemotePort", httpContext.Connection.RemotePort.ToString());
            dataBuilder.Replace("$IsHttps", httpContext.Request.IsHttps.ToString());
            return Task.FromResult(dataBuilder.ToString());
        }

        public async Task SendTemplate(HtcHttpContext httpContext) {
            httpContext.Response.ContentType = ContentType;
            await httpContext.Response.WriteAsync(await GetReplaced(httpContext));
        }
    }
}