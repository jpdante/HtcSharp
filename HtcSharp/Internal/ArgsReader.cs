using System.Collections.Generic;

namespace HtcSharp.Internal {
    public class ArgsReader {

        private readonly Dictionary<string, string> _argsDictionary;

        public ArgsReader(IReadOnlyList<string> args) {
            _argsDictionary = new Dictionary<string, string>();
            Load(args);
        }

        private void Load(IReadOnlyList<string> args) {
            var position = 0;
            var count = 0;
            while (position < args.Count) {
                string current = args[position];
                if (current.StartsWith("-")) {
                    if (position + 1 < args.Count) {
                        string value = args[position + 1];
                        if (!value.StartsWith("-")) {
                            _argsDictionary.Add(current.Remove(current.Length - 1, 1), value);
                            position++;
                        } else {
                            _argsDictionary.Add(current.Remove(current.Length - 1, 1), "1");
                        }
                    }
                } else {
                    _argsDictionary.Add(count.ToString(), current);
                    count++;
                }
                position++;
            }
        }

        public string GetOrDefault(string key, string defaultValue) {
            return _argsDictionary.TryGetValue(key, out string value) ? value : defaultValue;
        }

    }
}