using System;
using System.Collections.Generic;

namespace HtcSharp.HttpModule.Directive {
    public class DirectiveManager {

        private readonly Dictionary<string, Type> _directiveTypes;

        public DirectiveManager() {
            _directiveTypes = new Dictionary<string, Type>();
        }

        public void RegisterDirective(string name, Type type) => _directiveTypes.Add(name, type);

        public void RegisterDirective<T>(string name) => _directiveTypes.Add(name, typeof(T));

        public void UnregisterDirective(string name) => _directiveTypes.Remove(name);

        public bool TryGetDirective(string name, out Type directiveType) => _directiveTypes.TryGetValue(name, out directiveType);

    }
}