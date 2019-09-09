using System;
using System.IO;
using System.Reflection;
using HtcPlugin.Php.Processor.Models;
using HtcSharp.Core;
using HtcSharp.Core.Helpers.Http;
using HtcSharp.Core.Interfaces.Plugin;
using HtcSharp.Core.Logging;
using HtcSharp.Core.Models.Http;
using HtcSharp.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HtcPlugin.Php.Processor {
    public class PhpProcessor : IHtcPlugin, IHttpEvents {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);
        public static PhpProcessor Context;
        public string PluginName => "HtcPhpProcessor";
        public string PluginVersion => "0.3.7";
        public static string PhpCgiExec;
        public static string PhpPath;

        public PhpProcessor() {
            Context = this;
        }

        public void OnLoad() {
            var path = Path.Combine(HtcServer.Context.PluginsPath, @"PhpConfig.json");
            if (!File.Exists(path)) {
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                    using (var streamWriter = new StreamWriter(fileStream)) {
                        streamWriter.Write(JsonConvert.SerializeObject(new {
                            PhpCgiExec = "",
                            PhpPath = ""
                        }, Formatting.Indented));
                    }
                }
            }
            var config = IoUtils.GetJsonFile(path);
            PhpCgiExec = config.GetValue("PhpCgiExec", StringComparison.CurrentCultureIgnoreCase).Value<string>();
            PhpPath = config.GetValue("PhpPath", StringComparison.CurrentCultureIgnoreCase).Value<string>();
        }

        public void OnEnable() {
            UrlMapper.RegisterPluginExtension(".php", this);
            UrlMapper.RegisterIndexFile("index.php");
        }

        public void OnDisable() {
            UrlMapper.UnRegisterPluginExtension(".php");
            UrlMapper.UnRegisterIndexFile("index.php");
        }

        public bool OnHttpPageRequest(HtcHttpContext httpContext, string filename) {
            Logger.Warn($"A custom page was called. This should not happen! {{FileName: \"{filename}\"}}");
            httpContext.ErrorMessageManager.SendError(httpContext, 500);
            return false;
        }

        public bool OnHttpExtensionRequest(HtcHttpContext httpContext, string filename, string extension) {
            if (extension != ".php") return true;
            var response = PhpRequest.Request(httpContext, filename, 10000);
            return response;
        }
    }
}