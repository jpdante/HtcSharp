using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Abstractions.Routing;
using HtcSharp.HttpModule.Config;
using HtcSharp.HttpModule.Directive;
using HtcSharp.HttpModule.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HtcSharp.HttpModule.Routing {
    public class SiteLocation {

        private readonly DirectiveDelegate _directiveDelegate;

        public readonly Func<HttpContext, bool> Match;

        public SiteLocation(LocationConfig locationConfig, IServiceProvider serviceProvider) {
            Regex regex;
            switch (locationConfig.LocationType) {
                case LocationType.None:
                case LocationType.NoRegex:
                    Match = httpContext => httpContext.Request.Path.Value.StartsWith(locationConfig.Location);
                    break;
                case LocationType.Equal:
                    Match = httpContext => httpContext.Request.Path.Value.Equals(locationConfig.Location);
                    break;
                case LocationType.Regex:
                    regex = new Regex(locationConfig.Location);
                    Match = httpContext => regex.IsMatch(httpContext.Request.Path.Value);
                    break;
                case LocationType.RegexCaseInsensitive:
                    regex = new Regex(locationConfig.Location, RegexOptions.IgnoreCase);
                    Match = httpContext => regex.IsMatch(httpContext.Request.Path.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var directiveBuilder = new DirectiveBuilder(serviceProvider);
            var directiveManager = serviceProvider.GetService<DirectiveManager>();
            foreach (var directiveConfig in locationConfig.Directives) {
                if (!directiveConfig.TryGetProperty("type", out var property)) continue;
                string directiveName = property.GetString();
                if (string.IsNullOrEmpty(directiveName)) throw new NullReferenceException("Directive name is empty or invalid.");
                if (directiveManager.TryGetDirective(property.GetString(), out var directiveType)) {
                    directiveBuilder.UseDirective(directiveType, directiveConfig);
                } else {
                    throw new UnknownDirectiveException($"Unknown directive '{directiveName}', missing plugin?");
                }
            }
            _directiveDelegate = directiveBuilder.Build();
        }

        public async Task ProcessRequest(HtcHttpContext context) {
            await _directiveDelegate(context);
        }

    }
}