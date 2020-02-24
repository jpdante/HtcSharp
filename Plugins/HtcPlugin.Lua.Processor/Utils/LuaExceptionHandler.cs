using System;
using System.IO;
using System.Text;
using HtcSharp.HttpModule.Http.Abstractions;
using MoonSharp.Interpreter;

namespace HtcPlugin.Lua.Processor.Utils {
    public static class LuaExceptionHandler {
        
        public static void ErrorHeaderAlreadySent(HttpContext httpContext) {
            httpContext.Response.Body.Write(Encoding.UTF8.GetBytes(GenerateMessage("Failed to set the header, it has already been sent to the client!")));
        }

        public static void ErrorScriptRuntimeException(HttpContext httpContext, ScriptRuntimeException ex, string filepath) {
            if (ex.DecoratedMessage.Length <= 0) {
                httpContext.Response.Body.Write(Encoding.UTF8.GetBytes(GenerateMessage(ex.Message, "Fatal Error:")));
            } else {
                string fileName = Path.GetFileName(filepath);
                httpContext.Response.Body.Write(Encoding.UTF8.GetBytes(GenerateMessage(ex.DecoratedMessage.Replace(filepath, fileName), "Fatal Error:")));
            }
        }

        public static void ErrorUnknown(HttpContext httpContext, Exception ex) {
            httpContext.Response.Body.Write(Encoding.UTF8.GetBytes(GenerateMessage(ex.Message, "Fatal Error:")));
        }

        public static string GenerateMessage(string message, string type = "Error:") {
            return $"<p style=\"font-family: \'Lucida Sans Unicode\', \'Lucida Grande\', sans-serif\"><strong style=\"color: #C00D2F;\">{type}</strong> {message}</p>";
        }
    }
}
