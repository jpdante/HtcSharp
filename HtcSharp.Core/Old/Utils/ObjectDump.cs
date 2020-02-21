using System;
using System.Collections.Generic;
using System.Reflection;
using HtcSharp.Core.Old.Logging;

namespace HtcSharp.Core.Old.Utils {
    public static class ObjectDump {
        private static readonly Logger Logger = LogManager.GetILog(MethodBase.GetCurrentMethod().DeclaringType);

        public static void DumpToLogger(object obj) {
            var message = Environment.NewLine;
            if (obj == null) {
                message += $"Object is null{Environment.NewLine}";
                Logger.Info(message);
                return;
            }
            message += $"Name: {obj.GetType().Name}{Environment.NewLine}";
            message += $"Hash: {obj.GetHashCode()}{Environment.NewLine}";
            message += $"Type: {obj.GetType()}{Environment.NewLine}";
            var props = GetProperties(obj);
            if (props.Count > 0) {
                message += $"-------------------------{Environment.NewLine}";
            }
            foreach (var (key, value) in props) {
                message += $"{key}: {value}{Environment.NewLine}";
            }
            Logger.Info(message);
        }

        private static Dictionary<string, string> GetProperties(object obj) {
            var props = new Dictionary<string, string>();
            if (obj == null) return props;
            var type = obj.GetType();
            foreach (var prop in type.GetProperties()) {
                var val = prop.GetValue(obj, new object[] { });
                var valStr = val == null ? "" : val.ToString();
                props.Add(prop.Name, valStr);
            }

            return props;
        }
    }
}
