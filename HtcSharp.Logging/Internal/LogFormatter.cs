using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace HtcSharp.Logging.Internal {
    public class LogFormatter {

        private readonly string _format;

        public LogFormatter() {
            _format = "[%DD/%MM/%YYYY %hh:%mm:%ss] [%level] [%thread] %message %ex-message %obj\n%ex-stack";
        }

        public LogFormatter(string format) {
            _format = format;
        }

        public string FormatLog(LogLevel logLevel, string msg, params object[] objs) {
            var dateTime = DateTime.Now;
            var builder = new StringBuilder(_format);
            builder.Replace("%YYYY", dateTime.Year.ToString("0000"));
            builder.Replace("%MM", dateTime.Month.ToString("00"));
            builder.Replace("%DD", dateTime.Day.ToString("00"));
            builder.Replace("%hh", dateTime.Hour.ToString("00"));
            builder.Replace("%mm", dateTime.Minute.ToString("00"));
            builder.Replace("%ss", dateTime.Second.ToString("00"));
            builder.Replace("%ms", dateTime.Millisecond.ToString("00"));
            builder.Replace("%tt", dateTime.Ticks.ToString("00"));
            builder.Replace("%level", logLevel.ToString());
            builder.Replace("%thread", Thread.CurrentThread.ManagedThreadId.ToString());
            builder.Replace("%message", msg);
            builder.Replace("%ex-message", "");
            builder.Replace("%ex-stack", "");
            builder.Replace("%ex-hresult", "");
            builder.Replace("%ex-source", "");
            builder.Replace("%obj", objs is {Length: > 0} ? objs.Select(o => $"{o} ").Aggregate((i, j) => i + j) : "");
            return builder.ToString();
        }

        public string FormatLog(LogLevel logLevel, string msg, Exception ex, params object[] objs) {
            var dateTime = DateTime.Now;
            var builder = new StringBuilder(_format);
            builder.Replace("%YYYY", dateTime.Year.ToString("00"));
            builder.Replace("%MM", dateTime.Month.ToString("00"));
            builder.Replace("%DD", dateTime.Day.ToString("00"));
            builder.Replace("%hh", dateTime.Hour.ToString("00"));
            builder.Replace("%mm", dateTime.Minute.ToString("00"));
            builder.Replace("%ss", dateTime.Second.ToString("00"));
            builder.Replace("%ms", dateTime.Millisecond.ToString("00"));
            builder.Replace("%tt", dateTime.Ticks.ToString("00"));
            builder.Replace("%level", logLevel.ToString());
            builder.Replace("%thread", Thread.CurrentThread.ManagedThreadId.ToString());
            builder.Replace("%message", msg);
            builder.Replace("%ex-message", ex.Message);
            builder.Replace("%ex-stack", ex.StackTrace);
            builder.Replace("%ex-hresult", ex.HResult.ToString());
            builder.Replace("%ex-source", ex.Source);
            builder.Replace("%obj", objs is {Length: > 0} ? objs.Select(o => $"{o} ").Aggregate((i, j) => i + j) : "");
            return builder.ToString();
        }
    }
}