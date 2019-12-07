using System;

namespace HtcSharp.HttpModule2.Core.Infrastructure {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct, Inherited = false)]
    internal sealed class StackTraceHiddenAttribute : Attribute {
        public StackTraceHiddenAttribute() { }
    }
}