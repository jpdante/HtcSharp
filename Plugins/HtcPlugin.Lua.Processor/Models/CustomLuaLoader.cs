using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace HtcPlugin.Lua.Processor.Models {
    public class CustomLuaLoader : ScriptLoaderBase {

        private HashSet<string> _resourcesNamespaces;
        private List<Tuple<string, Assembly>> _resourcesAssemblies;

        public void Initialize() {
            _resourcesNamespaces = new HashSet<string>();
            _resourcesAssemblies = new List<Tuple<string, Assembly>>();
            foreach(var value in LuaProcessor.Context.LuaLowLevelAccess._lowLevelClasses.Values) {
                var assembly = value.GetType().Assembly;
                _resourcesNamespaces.Add(assembly.FullName.Split(',').First().Replace(".plugin", ""));
                foreach (var resourceName in assembly.GetManifestResourceNames()) {
                    _resourcesAssemblies.Add(new Tuple<string, Assembly>(resourceName, assembly));
                }
            }
        }

        private static string FileNameToResource(string resourceNamespace, string file) {
            file = file.Replace('/', '.');
            file = file.Replace('\\', '.');
            return resourceNamespace + "." + file;
        }

        public override bool ScriptFileExists(string name) {
            foreach (var resourceNamespace in _resourcesNamespaces) {
                var newName = FileNameToResource(resourceNamespace, name);
                foreach (var resourceAssembly in _resourcesAssemblies) {
                    if (resourceAssembly.Item1.Equals(newName, StringComparison.CurrentCultureIgnoreCase)) return true;
                }
            }
            return File.Exists(name);
        }

        public override object LoadFile(string file, Table globalContext) {
            foreach (var resourceNamespace in _resourcesNamespaces) {
                var newFile = FileNameToResource(resourceNamespace, file);
                foreach (var (item1, item2) in _resourcesAssemblies) {
                    if (item1.Equals(newFile, StringComparison.CurrentCultureIgnoreCase)) return item2.GetManifestResourceStream(newFile);
                }
            }
            return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}
