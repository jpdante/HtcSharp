using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.CoreLib;
using MoonSharp.Interpreter.Platforms;

namespace HtcPlugin.Lua.Processor.Core {
    public struct LuaContext {

        private readonly Script _script;
        private readonly HtcHttpContext _httpContext;
        private readonly string _fileName;
        private IFileInfo _fileInfo;

        public LuaContext(HtcHttpContext httpContext, string fileName) {
            _script = new Script();
            _httpContext = httpContext;
            _fileName = fileName;
            _fileInfo = null;
        }

        public bool LoadFile() {
            _fileInfo = _httpContext.Site.FileProvider.GetFileInfo(_fileName);
            return _fileInfo.Exists;
        }

        private void AddHeaders() {
            _script.Globals["_Header"] = _httpContext.Request.Headers.ToDictionary();
            _script.Globals["_Cookie"] = _httpContext.Request.Cookies.ToDictionary();
            _script.Globals["_Get"] = _httpContext.Request.Query.ToDictionary();
            if (_httpContext.Request.HasFormContentType) _script.Globals["_Post"] = _httpContext.Request.Form.ToDictionary();
            else _script.Globals["_Post"] = new Dictionary<string, string>();
            _script.Globals["_Server"] = new Dictionary<string, object>() {
                {"ServerName", Environment.MachineName},
                {"ServerAddr", _httpContext.Connection.LocalIpAddress.ToString()},
                {"ServerSoftware", "HtcSharp"},
                {"ServerProtocol",  _httpContext.Request.Protocol},
                {"RequestMethod", _httpContext.Request.Method},
                {"RequestTime", (int) new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()},
                {"RequestTimeFloat", (float) new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()},
                {"QueryString", _httpContext.Request.QueryString.ToString()},
                {"Https", _httpContext.Request.IsHttps},
                {"RemoteAddr", _httpContext.Connection.RemoteIpAddress.ToString()},
                {"RemotePort", _httpContext.Connection.RemotePort},
            };
        }

        private void AddFunctions() {
            // Status & ContentType
            _script.Globals["setstatus"] = new Action<int>(SetStatusCode);
            _script.Globals["setcontenttype"] = new Action<string>(SetContentType);
            // Headers
            _script.Globals["setheader"] = new Action<string, string>(SetHeader);
            _script.Globals["removeheader"] = new Action<string>(RemoveHeader);
            _script.Globals["clearheaders"] = new Action(ClearHeaders);
            // Cookies
            _script.Globals["setcookie"] = new Action<string, string>(SetCookie);
            _script.Globals["removecookie"] = new Action<string>(RemoveCookie);
            // Extras
            _script.Globals["redirect"] = new Action<string>(Redirect);
        }

        private void SetStatusCode(int status) {
            if (_httpContext.Response.HasStarted) throw new HeadersAlreadySentException();
            _httpContext.Response.StatusCode = status;
        }

        private void SetContentType(string contentType) {
            if (_httpContext.Response.HasStarted) throw new HeadersAlreadySentException();
            _httpContext.Response.ContentType = contentType;
        }

        private void Redirect(string url) {
            if (_httpContext.Response.HasStarted) throw new HeadersAlreadySentException();
            _httpContext.Response.Redirect(url);
        }

        private void SetHeader(string key, string value) {
            if (_httpContext.Response.HasStarted) throw new HeadersAlreadySentException();
            _httpContext.Response.Headers.Add(key, value);
        }

        private void RemoveHeader(string key) {
            if (_httpContext.Response.HasStarted) throw new HeadersAlreadySentException();
            _httpContext.Response.Headers.Remove(key);
        }

        private void ClearHeaders() {
            if (_httpContext.Response.HasStarted) throw new HeadersAlreadySentException();
            _httpContext.Response.Headers.Clear();
        }

        private void SetCookie(string key, string value) {
            if (_httpContext.Response.HasStarted) throw new HeadersAlreadySentException();
            _httpContext.Response.Cookies.Append(key, value);
        }

        private void RemoveCookie(string key) {
            if (_httpContext.Response.HasStarted) throw new HeadersAlreadySentException();
            _httpContext.Response.Cookies.Delete(key);
        }

        public async Task ProcessRequest() {
            var stream = _fileInfo.CreateReadStream();
            var httpContext = _httpContext;
            var script = _script;
            var finished = false;

            script.Options.DebugPrint = async s => {
                if (!finished) await httpContext.Response.WriteAsync(s);
            };

            _httpContext.Response.ContentType = ContentType.HTML.ToValue();
            _httpContext.Response.StatusCode = 200;

            AddHeaders();
            AddFunctions();

            IoModule.SetDefaultFile(script, StandardFileType.StdIn, httpContext.Request.Body);
            IoModule.SetDefaultFile(script, StandardFileType.StdOut, httpContext.Response.Body);
            IoModule.SetDefaultFile(script, StandardFileType.StdErr, httpContext.Response.Body);

            try {
                var scriptData = script.LoadStream(stream, codeFriendlyName: _fileInfo.Name);
                scriptData.Function.Call();
                script.DoString("io.flush()");
            } catch (HeadersAlreadySentException ex) {
                await _httpContext.Response.WriteAsync(GetFormatedString(ex.Message));
            } catch (SyntaxErrorException ex) {
                await _httpContext.Response.WriteAsync(GetFormatedString(ex.DecoratedMessage));
            } catch (InternalErrorException ex) {
                await _httpContext.Response.WriteAsync(GetFormatedString(ex.DecoratedMessage));
            } catch (DynamicExpressionException ex) {
                await _httpContext.Response.WriteAsync(GetFormatedString(ex.DecoratedMessage));
            } catch (ScriptRuntimeException ex) {
                await _httpContext.Response.WriteAsync(GetFormatedString(ex.DecoratedMessage));
            }

            finished = true;
        }

        private string GetFormatedString(string message) {
            return _httpContext.Response.ContentType == ContentType.HTML.ToValue() ? $"<p style=\"color: black; font-family: Arial, Helvetica, sans-serif\"><span style=\"display: inline; font-weight: 600; color: red\">Error:</span>&nbsp;{message}</p>" : message;
        }
    }
}