using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Extensions;
using MoonSharp.Interpreter;

namespace HtcPlugin.Lua.Processor.Utils {
    public static class LuaExceptionHandler {
        
        public static void ErrorHeaderAlreadySent(HttpContext httpContext) {
            httpContext.Response.WriteAsync(GenerateMessage("Failed to set the header, it has already been sent to the client!")).GetAwaiter().GetResult();
        }

        public static void ErrorScriptRuntimeException(HttpContext httpContext, ScriptRuntimeException ex, string filepath) {
            if (ex.DecoratedMessage.Length <= 0) {
                httpContext.Response.WriteAsync(GenerateMessage(ex.Message, "Fatal Error:")).GetAwaiter().GetResult();
            } else {
                string fileName = Path.GetFileName(filepath);
                httpContext.Response.WriteAsync(GenerateMessage(ex.DecoratedMessage.Replace(filepath, fileName), "Fatal Error:")).GetAwaiter().GetResult();
            }
        }

        public static void ErrorUnknown(HttpContext httpContext, Exception ex) {
            httpContext.Response.WriteAsync(GenerateMessage(ex.Message, "Fatal Error:")).GetAwaiter().GetResult();
        }

        public static string GenerateMessage(string message, string type = "Error:") {
            return $"<p style=\"font-family: \'Lucida Sans Unicode\', \'Lucida Grande\', sans-serif\"><strong style=\"color: #C00D2F;\">{type}</strong> {message}</p>";
        }
    }
}
