using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions.Routing;
using HtcSharp.HttpModule.Config;
using HtcSharp.HttpModule.Http;
using Microsoft.AspNetCore.Http;
using RequestDelegate = HtcSharp.HttpModule.Middleware.RequestDelegate;

namespace HtcSharp.HttpModule.Core {
    public class SiteLocation {

        private readonly RequestDelegate _requestDelegate;

        private readonly Func<HtcHttpContext, bool> _matchFunc;

        public SiteLocation(LocationConfig locationConfig) {
            Regex regex;
            switch (locationConfig.LocationType) {
                case LocationType.None:
                case LocationType.NoRegex:
                    _matchFunc = httpContext => httpContext.Request.Path.Value.StartsWith(locationConfig.Location);
                    break;
                case LocationType.Equal:
                    _matchFunc = httpContext => httpContext.Request.Path.Value.Equals(locationConfig.Location);
                    break;
                case LocationType.Regex:
                    regex = new Regex(locationConfig.Location);
                    _matchFunc = httpContext => regex.IsMatch(httpContext.Request.Path.Value);
                    break;
                case LocationType.RegexCaseInsensitive:
                    regex = new Regex(locationConfig.Location, RegexOptions.IgnoreCase);
                    _matchFunc = httpContext => regex.IsMatch(httpContext.Request.Path.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (string middlwareConfig in locationConfig.MiddlewaresConfig) {
                
            }
        }

        public bool Match(HtcHttpContext context) {
            return _matchFunc(context);
        }

        public async Task ProcessRequest(HtcHttpContext context) {
            await context.Response.WriteAsync("Location used!");
            await _requestDelegate(context);
        }

    }
}