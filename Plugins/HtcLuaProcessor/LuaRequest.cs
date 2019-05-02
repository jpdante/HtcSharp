using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Models.Http;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace HtcLuaProcessor {
    public class LuaRequest {
        public static bool Request(string filename, HtcHttpContext httpContext) {
            try {
                var headerSent = false;
                var statusCode = 200;
                var contentType = ContentType.HTML.ToValue();
                var luaScript = new Script {
                    Options = {
                        DebugPrint = data => {
                            // ReSharper disable once AccessToModifiedClosure
                            if (!headerSent) {
                                headerSent = true;
                                httpContext.Response.StatusCode = statusCode;
                                httpContext.Response.ContentType = contentType;
                            }
                            httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes(data));
                        }
                    }
                };
                ((ScriptLoaderBase) luaScript.Options.ScriptLoader).ModulePaths = GenerateModulesPath(filename);
                Action<string, string, string> SetCookieAction = (arg1, arg2, arg3) => {
                    if (headerSent) {
                        ErrorHeaderAlreadySent(httpContext);
                        return;
                    }
                    httpContext.Response.Cookies.Append(arg1, arg2, int.Parse(arg3));
                };
                Action<string> UnsetCookieAction = (arg1) => {
                    if (headerSent) {
                        ErrorHeaderAlreadySent(httpContext);
                        return;
                    }
                    httpContext.Response.Cookies.Delete(arg1);
                };
                Action<string> RedirectAction = (arg1) => {
                    if (headerSent) {
                        ErrorHeaderAlreadySent(httpContext);
                        return;
                    }
                    httpContext.Response.Redirect(arg1);
                };
                Action<int> SetStatusAction = (arg1) => {
                    if (headerSent) {
                        ErrorHeaderAlreadySent(httpContext);
                        return;
                    }
                    statusCode = arg1;
                    httpContext.Response.StatusCode = arg1;
                };
                Action<string, string> SetHeaderAction = (arg1, arg2) => {
                    if (headerSent) {
                        ErrorHeaderAlreadySent(httpContext);
                        return;
                    }
                    httpContext.Response.Headers.Add(arg1, arg2);
                };
                Action<string> SetContentTypeAction = (arg1) => {
                    if (headerSent) {
                        ErrorHeaderAlreadySent(httpContext);
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
                luaScript.DoFile(filename);
                if (!headerSent) {
                    headerSent = true;
                    httpContext.Response.StatusCode = statusCode;
                    httpContext.Response.ContentType = contentType;
                    httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes(""));
                }
            } catch (ScriptRuntimeException ex) {
                ErrorScriptRuntimeException(httpContext, ex, filename);
            } catch (Exception ex) {
                ErrorUnknown(httpContext, ex);
            }
            return false;
        }

        public static string[] GenerateModulesPath(string filename) {
            var luaIncludePath = Path.GetDirectoryName(filename).Replace(@"\", "/");
            return new[] { $"{luaIncludePath}/?", $"{luaIncludePath}/?.lua" };
        }

        public static void ErrorHeaderAlreadySent(HtcHttpContext httpContext) {
            httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes("<br><strong style=\"color: #d50000; font-family: Arial, Helvetica, sans-serif;\">[Lua] attempt to set the header but it has already been sent to the client!</strong><br>"));
        }

        public static void ErrorScriptRuntimeException(HtcHttpContext httpContext, ScriptRuntimeException ex, string filepath) {
            if (ex.DecoratedMessage.Length <= 0) {
                httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes($"<br><strong style=\"color: #d50000; font-family: Arial, Helvetica, sans-serif;\">[Lua] {ex.Message}</strong><br>"));
            } else {
                var fileName = Path.GetFileName(filepath);
                httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes($"<br><strong style=\"color: #d50000; font-family: Arial, Helvetica, sans-serif;\">[Lua] {ex.DecoratedMessage.Replace(filepath, fileName)}</strong><br>"));
            }
        }

        public static void ErrorUnknown(HtcHttpContext httpContext, Exception ex) {
            httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes($"<br><strong style=\"color: #d50000; font-family: Arial, Helvetica, sans-serif;\">[Lua] exception occurred => {ex.Message}</strong><br>"));
        }
    }
}