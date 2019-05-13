using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HtcSharp.Core.Models.Http;
using MoonSharp.Interpreter;

namespace HtcPlugin.LuaProcessor.Utils {
    public static class LuaExceptionHandler {
        
        public static void ErrorHeaderAlreadySent(HtcHttpContext httpContext) {
            httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes(GenerateMessage("Failed to set the header, it has already been sent to the client!")));
        }

        public static void ErrorScriptRuntimeException(HtcHttpContext httpContext, ScriptRuntimeException ex, string filepath) {
            if (ex.DecoratedMessage.Length <= 0) {
                httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes(GenerateMessage(ex.Message, "Fatal Error:")));
            } else {
                var fileName = Path.GetFileName(filepath);
                httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes(GenerateMessage(ex.DecoratedMessage.Replace(filepath, fileName), "Fatal Error:")));
            }
        }

        public static void ErrorUnknown(HtcHttpContext httpContext, Exception ex) {
            httpContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes(GenerateMessage(ex.Message, "Fatal Error:")));
        }

        public static string GenerateMessage(string message, string type = "Error:") {
            return $"<p style=\"font-family: \'Lucida Sans Unicode\', \'Lucida Grande\', sans-serif\"><strong style=\"color: #C00D2F;\">{type}</strong> {message}</p>";
        }
    }
}
