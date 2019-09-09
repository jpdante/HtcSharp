using System;
using System.Collections.Generic;
using System.Text;
using HtcPlugin.Lua.Processor.Utils;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Models.Http;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace HtcPlugin.Lua.Processor.Models {
    public abstract class LuaPageRequest {

        public bool Request(HtcHttpContext httpContext, Script luaScript, string filename) {
            try {
                var headerSent = false;
                var statusCode = 200;
                var contentType = ContentType.HTML.ToValue();
                luaScript.Options.DebugPrint = data => {
                    if (!headerSent) {
                        headerSent = true;
                        httpContext.Response.StatusCode = statusCode;
                        httpContext.Response.ContentType = contentType;
                    }
                    httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes(data));
                };
                Action<string, string, string> SetCookieAction = (arg1, arg2, arg3) => {
                    if (headerSent) {
                        LuaExceptionHandler.ErrorHeaderAlreadySent(httpContext);
                        return;
                    }
                    httpContext.Response.Cookies.Append(arg1, arg2, int.Parse(arg3));
                };
                Action<string> UnsetCookieAction = (arg1) => {
                    if (headerSent) {
                        LuaExceptionHandler.ErrorHeaderAlreadySent(httpContext);
                        return;
                    }
                    httpContext.Response.Cookies.Delete(arg1);
                };
                Action<string> RedirectAction = (arg1) => {
                    if (headerSent) {
                        LuaExceptionHandler.ErrorHeaderAlreadySent(httpContext);
                        return;
                    }
                    httpContext.Response.Redirect(arg1);
                };
                Action<int> SetStatusAction = (arg1) => {
                    if (headerSent) {
                        LuaExceptionHandler.ErrorHeaderAlreadySent(httpContext);
                        return;
                    }
                    statusCode = arg1;
                    httpContext.Response.StatusCode = arg1;
                };
                Action<string, string> SetHeaderAction = (arg1, arg2) => {
                    if (headerSent) {
                        LuaExceptionHandler.ErrorHeaderAlreadySent(httpContext);
                        return;
                    }
                    httpContext.Response.Headers.Add(arg1, arg2);
                };
                Action<string> SetContentTypeAction = (arg1) => {
                    if (headerSent) {
                        LuaExceptionHandler.ErrorHeaderAlreadySent(httpContext);
                        return;
                    }
                    contentType = arg1;
                    httpContext.Response.ContentType = arg1;
                };
                luaScript.Globals["setcookie"] = SetCookieAction;
                luaScript.Globals["unsetcookie"] = UnsetCookieAction;
                luaScript.Globals["redirect"] = RedirectAction;
                luaScript.Globals["setstatus"] = SetStatusAction;
                luaScript.Globals["setheader"] = SetHeaderAction;
                luaScript.Globals["setcontenttype"] = SetContentTypeAction;
                luaScript.Globals["_HEADER"] = httpContext.Request.Headers;
                luaScript.Globals["_COOKIE"] = httpContext.Request.Cookies;
                luaScript.Globals["_POST"] = httpContext.Request.Post;
                luaScript.Globals["_GET"] = httpContext.Request.Query;
                luaScript.Globals["_SERVER"] = new Dictionary<string, object>() {
                    {"SERVER_NAME", Environment.MachineName},
                    {"SERVER_ADDR", httpContext.Connection.LocalIpAddress.ToString()},
                    {"SERVER_SOFTWARE", "HtcSharp"},
                    {"SERVER_PROTOCOL",  httpContext.Request.Protocol},
                    {"REQUEST_METHOD", httpContext.Request.Method.ToString()},
                    {"REQUEST_TIME", httpContext.Request.RequestTimestamp},
                    {"REQUEST_TIME_FLOAT", (float)httpContext.Request.RequestTimestampMs},
                    {"QUERY_STRING", httpContext.Request.QueryString},
                    {"HTTPS", httpContext.Request.IsHttps},
                    {"REMOTE_ADDR", httpContext.Connection.RemoteIpAddress.ToString()},
                    {"REMOTE_PORT", httpContext.Connection.RemotePort},
                };
                if (!BeforeLuaRequest(httpContext, luaScript, filename)) {
                    luaScript.DoFile(filename);
                    if (!headerSent)
                    {
                        headerSent = true;
                        httpContext.Response.StatusCode = statusCode;
                        httpContext.Response.ContentType = contentType;
                        httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes(""));
                    }
                }
            } catch (ScriptRuntimeException ex) {
                LuaExceptionHandler.ErrorScriptRuntimeException(httpContext, ex, filename);
            } catch (Exception ex) {
                LuaExceptionHandler.ErrorUnknown(httpContext, ex);
            }
            return AfterLuaRequest(httpContext, luaScript, filename);
        }

        public abstract bool BeforeLuaRequest(HtcHttpContext httpContext, Script luaScript, string filename);

        public abstract bool AfterLuaRequest(HtcHttpContext httpContext, Script luaScript, string filename);

        public static Script NewScript() => new Script();
    }
}