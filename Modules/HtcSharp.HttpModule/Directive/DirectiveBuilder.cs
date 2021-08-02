using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using Microsoft.AspNetCore.Http;

namespace HtcSharp.HttpModule.Directive {
    public class DirectiveBuilder : IDirectiveBuilder {

        public IServiceProvider ApplicationServices { get; }

        private readonly IList<Func<DirectiveDelegate, DirectiveDelegate>> _directives;

        public DirectiveBuilder(IServiceProvider serviceProvider) {
            ApplicationServices = serviceProvider;
            _directives = new List<Func<DirectiveDelegate, DirectiveDelegate>>();
        }

        public IDirectiveBuilder Use(Func<DirectiveDelegate, DirectiveDelegate> middleware) {
            _directives.Add(middleware);
            return this;
        }

        public DirectiveDelegate Build() {
            DirectiveDelegate app = context =>  {
                // End of pipe-line
                context.Response.StatusCode = 404;
                return context.Site.TemplateManager.TryGetTemplate("404", out var template) ? template.SendTemplate(context) : Task.CompletedTask;
            };
            return _directives.Reverse().Aggregate(app, (current, component) => component(current));
        }

    }
}