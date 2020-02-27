using System;
using System.IO;
using System.Threading.Tasks;
using HtcPlugin.Php.Processor.Models;
using HtcSharp.Core;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.Core.Plugin;
using HtcSharp.Core.Plugin.Abstractions;
using HtcSharp.Core.Utils;
using HtcSharp.HttpModule;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Routing;
using Newtonsoft.Json;

namespace HtcPlugin.Php.Processor {
    public class PhpProcessor : IPlugin, IHttpEvents {

        public string Name => "HtcPhpProcessor";
        public string Version => "0.3.7";

        public static PhpProcessor Context;
        public static string PhpCgiExec;
        public static string PhpPath;
        public static int Timeout;

        public ILogger Logger;

        public PhpProcessor() {
            Context = this;
        }

        public Task Load(PluginServerContext pluginServerContext, ILogger logger) {
            Logger = logger;
            string path = Path.Combine(pluginServerContext.PluginsPath, @"PhpConfig.json");
            if (!File.Exists(path)) {
                using var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                using var streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(JsonConvert.SerializeObject(new {
                    PhpCgiExec = "%WorkingPath%\\plugins\\php\\php-cgi.exe",
                    PhpPath = "%WorkingPath%\\plugins\\php",
                    Timeout = 10000
                }, Formatting.Indented));
            }
            var config = JsonUtils.GetJsonFile(path);
            PhpCgiExec = HtcIOUtils.ReplacePathTags(config.GetValue("PhpCgiExec", StringComparison.CurrentCultureIgnoreCase).ToObject<string>());
            PhpPath = HtcIOUtils.ReplacePathTags(config.GetValue("PhpPath", StringComparison.CurrentCultureIgnoreCase).ToObject<string>());
            Timeout = config.GetValue("Timeout", StringComparison.CurrentCultureIgnoreCase).ToObject<int>();
            return Task.CompletedTask;
        }

        public Task Enable() {
            UrlMapper.RegisterPluginExtension(".php", this);
            UrlMapper.RegisterIndexFile("index.php");
            return Task.CompletedTask;
        }

        public Task Disable() {
            UrlMapper.UnRegisterPluginExtension(".php");
            UrlMapper.UnRegisterIndexFile("index.php");
            return Task.CompletedTask;
        }

        public bool IsCompatible(int htcMajor, int htcMinor, int htcPatch) {
            return true;
        }

        public async Task OnHttpPageRequest(HttpContext httpContext, string filename) {
            Logger.LogWarn($"A custom page was called. This should not happen! {{FileName: \"{filename}\"}}");
            await httpContext.ServerInfo.ErrorMessageManager.SendError(httpContext, 500);
        }

        public async Task OnHttpExtensionRequest(HttpContext httpContext, string filename, string extension) {
            if (extension != ".php") return;
            await PhpRequest.Request(httpContext, filename, Timeout);
            return;
        }
    }
}