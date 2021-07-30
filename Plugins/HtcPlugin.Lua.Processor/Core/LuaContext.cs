using System.IO;
using System.Text;
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

        public bool Load() {
            _fileInfo = _httpContext.Site.FileProvider.GetFileInfo(_fileName);
            return _fileInfo.Exists;
        }

        public async Task ProcessRequest() {
            var stream = _fileInfo.CreateReadStream();
            var httpContext = _httpContext;
            var script = _script;

            await using var memoryStream = new MemoryStream();

            IoModule.SetDefaultFile(script, StandardFileType.StdIn, httpContext.Request.Body);
            IoModule.SetDefaultFile(script, StandardFileType.StdOut, memoryStream);
            IoModule.SetDefaultFile(script, StandardFileType.StdErr, memoryStream);

            try {
                var scriptData = script.LoadStream(stream);
                scriptData.Function.Call();
                script.DoString("io.flush()");
            } catch (SyntaxErrorException ex) {
                await _httpContext.Response.WriteAsync(ex.DecoratedMessage);
            } catch (InternalErrorException ex) {
                await _httpContext.Response.WriteAsync(ex.DecoratedMessage);
            } catch (DynamicExpressionException ex) {
                await _httpContext.Response.WriteAsync(ex.DecoratedMessage);
            } catch (ScriptRuntimeException ex) {
                await _httpContext.Response.WriteAsync(ex.DecoratedMessage);
            }

            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(_httpContext.Response.Body);
        }
    }
}