using Newtonsoft.Json;

namespace HtcSharp.Core.Old.Utils {
    public static class JsonUtils {
        public static string SerializeObject(object obj) {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
