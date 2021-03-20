using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace HtcPlugin.Lua.Processor.Models {
    public class CustomLuaLoader : ScriptLoaderBase {

        private HashSet<string> _resourcesNamespaces;
        private List<Tuple<string, Assembly>> _resourcesAssemblies;

        public void Initialize() {
            _resourcesNamespaces = new HashSet<string>();
            _resourcesAssemblies = new List<Tuple<string, Assembly>>();
            foreach (var assembly in LuaProcessor.Context.LuaLowLevelAccess._lowLevelClasses.Values.Select(value => value.GetType().Assembly)) {
                _resourcesNamespaces.Add(assembly.FullName.Split(',').First().Replace(".plugin", ""));
                foreach (string resourceName in assembly.GetManifestResourceNames()) {
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
            return (from resourceNamespace in _resourcesNamespaces select FileNameToResource(resourceNamespace, name) into newName from resourceAssembly in _resourcesAssemblies where resourceAssembly.Item1.Equals(newName, StringComparison.CurrentCultureIgnoreCase) select newName).Any() || File.Exists(name);
        }

        public override object LoadFile(string file, Table globalContext) {
            foreach (string newFile in _resourcesNamespaces.Select(resourceNamespace => FileNameToResource(resourceNamespace, file))) {
                foreach ((string item1, var item2) in _resourcesAssemblies) {
                    Console.WriteLine(item1);
                    if (item1.Equals(newFile, StringComparison.CurrentCultureIgnoreCase)) return item2.GetManifestResourceStream(newFile);
                }
            }

            return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}
