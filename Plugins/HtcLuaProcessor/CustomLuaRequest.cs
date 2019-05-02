using System;
using System.IO;
using System.IO.Enumeration;
using System.Text;
using HtcSharp.Core.Models.Http;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace HtcLuaProcessor {
    public class CustomLuaRequest {

        private readonly string _luaFilename;
        private readonly Script _luaScript;
        private readonly HtcHttpContext _httpContext;
        private readonly DynValue _dynScript;

        public CustomLuaRequest(string luaFilename, HtcHttpContext httpContext) {
            _luaFilename = luaFilename;
            _httpContext = httpContext;
            _luaScript = new Script();
            var luaIncludePath = Path.GetDirectoryName(_luaFilename).Replace(@"\", "/");
            ((ScriptLoaderBase)_luaScript.Options.ScriptLoader).ModulePaths = new string[] { $"{luaIncludePath}/?", $"{luaIncludePath}/?.lua" };
            _dynScript = _luaScript.LoadFile(_luaFilename);
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
        
        public static void ErrorHeaderAlreadySent(HtcHttpContext httpContext) {
            httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes("<br><strong style=\"color: #d50000; font-family: Arial, Helvetica, sans-serif;\">[Lua] attempt to set the header but it has already been sent to the client!</strong><br>"));
        }

        public static void ErrorScriptRuntimeException(HtcHttpContext httpContext, ScriptRuntimeException ex, string filepath) {
            if(ex.DecoratedMessage.Length == 0) {
                httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes($"<br><strong style=\"color: #d50000; font-family: Arial, Helvetica, sans-serif;\">[Lua] {ex.Message}</strong><br>"));
            } else {
                var luaPath = ex.DecoratedMessage.Split(":(")[0];
                var fileName = Path.GetFileName(filepath);
                httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes($"<br><strong style=\"color: #d50000; font-family: Arial, Helvetica, sans-serif;\">[Lua] {ex.DecoratedMessage.Replace(filepath, fileName)}</strong><br>"));
            }
        }

        public static void ErrorUnknown(HtcHttpContext httpContext, Exception ex) {
            httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes($"<br><strong style=\"color: #d50000; font-family: Arial, Helvetica, sans-serif;\">[Lua] exception occurred => {ex.Message}</strong><br>"));
        }
        
    }
}