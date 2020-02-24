using System;
using System.IO;
using System.Text;
using HtcSharp.HttpModule.Http.Abstractions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace HtcPlugin.Lua.Processor {
    public class CustomLuaRequest {
        private readonly Script _luaScript;
        private readonly HttpContext _httpContext;
        private bool _headerSent = false;
        private readonly DynValue _dynScript;

        public CustomLuaRequest(string luaFilename, HttpContext httpContext) {
            string luaFilename1 = luaFilename;
            _httpContext = httpContext;
            _luaScript = new Script();
            var luaIncludePath = Path.GetDirectoryName(luaFilename1).Replace(@"\", "/");
            ((ScriptLoaderBase)_luaScript.Options.ScriptLoader).ModulePaths = new string[] { $"{luaIncludePath}/?", $"{luaIncludePath}/?.lua" };
            _luaScript.Options.DebugPrint = data => {
                if (!_headerSent) {
                    _headerSent = true;
                    //httpContext.Response.StatusCode = statusCode;
                    //httpContext.Response.ContentType = contentType;
                }
                httpContext.Response.Body.Write(Encoding.UTF8.GetBytes(data));
            };
            _dynScript = _luaScript.LoadFile(luaFilename1);
        }

        public bool Request() {
            _luaScript.Call(_dynScript);
            return false;
        }

        public void RegisterLuaCommand(string key, Action action) => _luaScript.Globals[key] = action;
        public DynValue CallFunction(string key, params object[] args) => _luaScript.Call(_luaScript.Globals[key], args);
        public DynValue GetValue(object key) => _luaScript.Globals.Get(key);
        public DynValue GetValues(params object[] key) => _luaScript.Globals.Get(key);
        public void SetValue(object key, DynValue value) => _luaScript.Globals.Set(key, value);
        public void AppendValue(DynValue value) => _luaScript.Globals.Append(value);
        public void RemoveValue(DynValue key) => _luaScript.Globals.Remove(key);
        public void ClearValues() => _luaScript.Globals.Clear();
        
        public static void ErrorHeaderAlreadySent(HttpContext httpContext) {
            httpContext.Response.Body.Write(Encoding.UTF8.GetBytes("<br><strong style=\"color: #d50000; font-family: Arial, Helvetica, sans-serif;\">[Lua] attempt to set the header but it has already been sent to the client!</strong><br>"));
        }

        public static void ErrorScriptRuntimeException(HttpContext httpContext, ScriptRuntimeException ex, string filepath) {
            if(ex.DecoratedMessage.Length == 0) {
                httpContext.Response.Body.Write(Encoding.UTF8.GetBytes($"<br><strong style=\"color: #d50000; font-family: Arial, Helvetica, sans-serif;\">[Lua] {ex.Message}</strong><br>"));
            } else {
                string luaPath = ex.DecoratedMessage.Split(":(")[0];
                string fileName = Path.GetFileName(filepath);
                httpContext.Response.Body.Write(Encoding.UTF8.GetBytes($"<br><strong style=\"color: #d50000; font-family: Arial, Helvetica, sans-serif;\">[Lua] {ex.DecoratedMessage.Replace(filepath, fileName)}</strong><br>"));
            }
        }

        public static void ErrorUnknown(HttpContext httpContext, Exception ex) {
            httpContext.Response.Body.Write(Encoding.UTF8.GetBytes($"<br><strong style=\"color: #d50000; font-family: Arial, Helvetica, sans-serif;\">[Lua] exception occurred => {ex.Message}</strong><br>"));
        }
        
    }
}