using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HtcSharp.Core.Utils {
    public static class JsonUtils {
        public static string SerializeObject(object obj) {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
