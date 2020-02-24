using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HtcSharp.Core.Utils {
    public static class JsonUtils {

        public static JObject GetJsonFile(string filename) {
            using var file = File.OpenText(filename);
            using var reader = new JsonTextReader(file);
            var data = (JObject)JToken.ReadFrom(reader);
            return data;
        }
    }
}
