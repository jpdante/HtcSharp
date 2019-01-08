using HTCSharp.Core.Models.Http;
using HTCSharp.Core.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HTCSharp.Core.Models.Rewriter {
    public class FolderConfig {
        public string LastModification { get; set; }
        public string ConfigFileName { get; set; }
        public string ServerPath { get; set; }

        #region RewriteEngine
        public bool RewriteEnabled { get; set; }
        public List<Regex> RewriteConditions { get; set; }
        public string RewriteRule { get; set; }
        #endregion

        public FolderConfig() { }

        public FolderConfig(string serverPath, string configFileName) {
            Configure(serverPath, configFileName);
        }

        public string Rewrite(string request, HTCHttpContext context) {
            if (RewriteEnabled) {
                bool matchAll = true;
                foreach(Regex regex in RewriteConditions) {
                    if(!regex.IsMatch(request)) {
                        Console.WriteLine($"{regex.ToString()} -> {request} -> false");
                        matchAll = false;
                    } else {
                        Console.WriteLine($"{regex.ToString()} -> {request} -> true");
                    }
                }
                if(matchAll) {
                    string[] separatedPath = request.Split("/");
                    string[] rules = RewriteRule.Split(" ");
                    for (int p = 1; p < separatedPath.Length; p++) {
                        request = rules[0].Replace($"${p}", separatedPath[p]);
                        for (int r = 1; r < rules.Length; r++) {
                            //Console.WriteLine($"Path Part -> {separatedPath[p]}");
                            string[] query = rules[r].Split("=");
                            //Console.WriteLine($"Exec Rule -> {rules[r]}");
                            //Console.WriteLine($"GET Inject -> {query[0]}={query[1].Replace($"${p}", separatedPath[p])}");
                            context.Request.Query.Inject(query[0], query[1].Replace($"${p}", separatedPath[p]));
                        }
                    }
                }
            }
            return request;
        }

        public bool CheckConfigChange() {
            if(!LastModification.Equals(File.GetLastWriteTime(ConfigFileName).ToString("r"), StringComparison.CurrentCultureIgnoreCase)) {
                Configure(ServerPath, ConfigFileName);
                return true;
            } else {
                return false;
            }
        }

        public void Configure(string serverPath, string configFileName) {
            ServerPath = serverPath;
            ConfigFileName = configFileName;
            LastModification = File.GetLastWriteTime(ConfigFileName).ToString("r");
            JObject config = IOUtils.GetJsonFile(ConfigFileName);
            JObject rewriterConfig = config.GetValue("HTCRewrite", StringComparison.CurrentCultureIgnoreCase)?.ToObject<JObject>();
            if (rewriterConfig == null) {
                RewriteEnabled = false;
            } else {
                RewriteEnabled = true;
                RewriteConditions = new List<Regex>();
                JArray conditions = rewriterConfig.GetValue("Conditions", StringComparison.CurrentCultureIgnoreCase)?.ToObject<JArray>();
                if(conditions != null) {
                    foreach(JToken value in conditions) {
                        RewriteConditions.Add(new Regex(value.ToObject<string>()));
                    }
                }
                string rule = rewriterConfig.GetValue("Rule", StringComparison.CurrentCultureIgnoreCase)?.ToObject<string>();
                if(rule != null) {
                    RewriteEnabled = true;
                    RewriteRule = rule;
                } else {
                    RewriteEnabled = false;
                }
            }
        }

    }
}
