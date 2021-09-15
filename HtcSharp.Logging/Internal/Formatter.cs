using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace HtcSharp.Logging.Internal {
    public class Formatter : IFormatter {

        private readonly string _format;

        public Formatter() {
            _format = "[%DD/%MM/%YYYY %hh:%mm:%ss.%ms] [%level] [%thread] [%type] %message %ex-message %obj\n%ex-stack";
        }

        public Formatter(string format) {
            _format = format;
        }

        public string FormatLog(ILogger logger, LogLevel logLevel, object msg, Exception ex, params object[] objs) {
            var dateTime = DateTime.Now;
            var builder = new StringBuilder(_format);
            builder.Replace("%YYYY", dateTime.Year.ToString("00"));
            builder.Replace("%MM", dateTime.Month.ToString("00"));
            builder.Replace("%DD", dateTime.Day.ToString("00"));
            builder.Replace("%hh", dateTime.Hour.ToString("00"));
            builder.Replace("%mm", dateTime.Minute.ToString("00"));
            builder.Replace("%ss", dateTime.Second.ToString("00"));
            builder.Replace("%ms", dateTime.Millisecond.ToString("0"));
            builder.Replace("%tt", dateTime.Ticks.ToString("00"));
            builder.Replace("%level", logLevel.ToString());
            builder.Replace("%thread", Thread.CurrentThread.ManagedThreadId.ToString());
            builder.Replace("%message", msg?.ToString());
            if (ex == null) {
                builder.Replace("%ex-message", null);
                builder.Replace("%ex-stack", null);
                builder.Replace("%ex-hresult", null);
                builder.Replace("%ex-source", null);
            } else {
                builder.Replace("%ex-message", ex.Message);
                builder.Replace("%ex-stack", $"{ex.StackTrace}\n");
                builder.Replace("%ex-hresult", ex.HResult.ToString());
                builder.Replace("%ex-source", ex.Source);
            }
            builder.Replace("%type", logger.Type.Name);
            builder.Replace("%fulltype", logger.Type.FullName);
            builder.Replace("%obj", objs is {Length: > 0} ? objs.Select(o => $"{o} ").Aggregate((i, j) => i + j) : "");
            return builder.ToString();
        }
    }
}