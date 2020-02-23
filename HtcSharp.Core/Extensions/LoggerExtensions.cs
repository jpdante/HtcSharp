using System;
using System.Collections.Generic;
using System.Text;
using HtcSharp.Core.Logging.Abstractions;

namespace HtcSharp.Core.Extensions {
    public static class LoggerExtensions {

        public static void DumpLog(this ILogger logger, LogLevel logLevel, object obj, Exception ex) {
            logger.Log(logLevel, GetDump(obj), ex);
        }

        public static string GetDump(object obj) {
            string message = Environment.NewLine;
            if (obj == null) {
                message += $"Object is null{Environment.NewLine}";
                return message;
            }
            message += $"Name: {obj.GetType().Name}{Environment.NewLine}";
            message += $"Hash: {obj.GetHashCode()}{Environment.NewLine}";
            message += $"Type: {obj.GetType()}{Environment.NewLine}";
            Dictionary<string, string> props = GetProperties(obj);
            if (props.Count > 0) {
                message += $"-------------------------{Environment.NewLine}";
            }
            foreach (var (key, value) in props) {
                message += $"{key}: {value}{Environment.NewLine}";
            }
            return message;
        }

        private static Dictionary<string, string> GetProperties(object obj) {
            var props = new Dictionary<string, string>();
            if (obj == null) return props;
            var type = obj.GetType();
            foreach (var prop in type.GetProperties()) {
                var val = prop.GetValue(obj, new object[] { });
                string valStr = val == null ? "" : val.ToString();
                props.Add(prop.Name, valStr);
            }
            return props;
        }

    }
}
