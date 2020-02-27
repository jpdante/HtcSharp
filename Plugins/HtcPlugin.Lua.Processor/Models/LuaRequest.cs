using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtcPlugin.Lua.Processor.Utils;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Features;
using HtcSharp.HttpModule.Routing;
using MoonSharp.Interpreter;

namespace HtcPlugin.Lua.Processor.Models {
    public class LuaRequest {

        public static Task Request(HttpContext httpContext, Script luaScript, string filename) {
            try {
                var headerSent = false;
                var statusCode = 200;
                string contentType = ContentType.HTML.ToValue();
                luaScript.Options.DebugPrint = async data => {
                    if (!headerSent) {
                        headerSent = true;
                        httpContext.Response.StatusCode = statusCode;
                        httpContext.Response.ContentType = contentType;
                    }
                    await httpContext.Response.WriteAsync(data);
                };
                Action<string, string, string> SetCookieAction = (arg1, arg2, arg3) => {
                    if (headerSent) {
                        LuaExceptionHandler.ErrorHeaderAlreadySent(httpContext);
                        return;
                    }
                    httpContext.Response.Cookies.Append(arg1, arg2, new CookieOptions() {
                        Expires = new DateTimeOffset().AddSeconds(int.Parse(arg3))
                    });
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
                luaScript.Globals["_POST"] = httpContext.Request.Form;
                luaScript.Globals["_GET"] = httpContext.Request.Query;
                luaScript.Globals["_SERVER"] = new Dictionary<string, object>() {
                    {"SERVER_NAME", Environment.MachineName},
                    {"SERVER_ADDR", httpContext.Connection.LocalIpAddress.ToString()},
                    {"SERVER_SOFTWARE", "HtcSharp"},
                    {"SERVER_PROTOCOL",  httpContext.Request.Protocol},
                    {"REQUEST_METHOD", httpContext.Request.Method},
                    {"REQUEST_TIME", (int)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()},
                    {"REQUEST_TIME_FLOAT", (float)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()},
                    {"QUERY_STRING", httpContext.Request.QueryString},
                    {"HTTPS", httpContext.Request.IsHttps},
                    {"REMOTE_ADDR", httpContext.Connection.RemoteIpAddress.ToString()},
                    {"REMOTE_PORT", httpContext.Connection.RemotePort},
                };
                luaScript.DoFile(filename);
                if (!headerSent) {
                    headerSent = true;
                    httpContext.Response.StatusCode = statusCode;
                    httpContext.Response.ContentType = contentType;
                    httpContext.Response.Body.Write(Encoding.UTF8.GetBytes(""));
                }
            } catch (ScriptRuntimeException ex) {
                LuaExceptionHandler.ErrorScriptRuntimeException(httpContext, ex, filename);
            } catch (Exception ex) {
                LuaExceptionHandler.ErrorUnknown(httpContext, ex);
            }
            return Task.CompletedTask;
        }

        public static Script NewScript() => new Script();
    }
}