using System;
using HtcSharp.HttpModule.Directive;

namespace HtcSharp.HttpModule.Abstractions {
    public interface IDirectiveBuilder {

        public IServiceProvider ApplicationServices { get; }

        public IDirectiveBuilder Use(Func<DirectiveDelegate, DirectiveDelegate> directives);

        public DirectiveDelegate Build();

    }
}