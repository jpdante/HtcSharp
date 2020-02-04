using System.Diagnostics;
using HtcSharp.HttpModule.Infrastructure.Interfaces;

namespace HtcSharp.HttpModule.Infrastructure {
    internal sealed class DebuggerWrapper : IDebugger {
        private DebuggerWrapper() { }

        public static IDebugger Singleton { get; } = new DebuggerWrapper();

        public bool IsAttached => Debugger.IsAttached;
    }
}