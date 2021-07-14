using HtcSharp.Logging.Internal;

namespace HtcSharp.Logging.Config {
    public class FormatterConfig {
        
        public string Format { get; set; } = null;

        public IFormatter GetFormatter() {
            return Format == null ? new Formatter() : new Formatter(Format);
        }
    }
}