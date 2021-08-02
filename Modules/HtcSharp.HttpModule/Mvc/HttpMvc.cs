using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions.Mvc;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Mvc.Exceptions;
using HtcSharp.HttpModule.Mvc.Parsers;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Http;

namespace HtcSharp.HttpModule.Mvc {
    public class HttpMvc {
        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private string _prefix;
        private bool _matchAnyDomain;
        private readonly HashSet<string> _domains;
        private readonly Dictionary<string, List<RouteContext>> _routes;
        private readonly Dictionary<Type, IHttpObjectParser> _parsers;

        public string Prefix => _prefix;
        public bool MatchAnyDomain => _matchAnyDomain;
        public IReadOnlySet<string> Domains => _domains;
        public IEnumerable<string> Routes => _routes.Keys;
        public IReadOnlyDictionary<Type, IHttpObjectParser> Parsers => _parsers;

        protected HttpMvc() {
            _prefix = null;
            _matchAnyDomain = true;
            _domains = new HashSet<string>();
            _routes = new Dictionary<string, List<RouteContext>>();
            _parsers = new Dictionary<Type, IHttpObjectParser> {{typeof(IHttpJsonObject), new JsonObjectParser()}, {typeof(IHttpXmlObject), new XmlObjectParser()}};
        }

        public HttpMvc LoadControllers(Assembly assembly) {
            foreach (var type in assembly.GetTypes()) {
                LoadController(type);
            }

            return this;
        }

        public HttpMvc LoadController(Type type) {
            IEnumerable<MethodInfo> functions = type.GetMethods().Where(m => m.GetCustomAttributes(typeof(HttpMethodAttribute), false).Length > 0);
            foreach (var function in functions) {
                if (function.ReturnType != typeof(Task) || !function.IsStatic) continue;

                ParameterInfo[] parameters = function.GetParameters();

                if (parameters.Length < 1 || parameters[0].ParameterType != typeof(HtcHttpContext)) continue;

                // Load routes
                var routeContexts = new List<RouteContext>();
                foreach (var attribute in function.GetCustomAttributes()) {
                    if (attribute is HttpMethodAttribute httpMethod) {
                        routeContexts.Add(new RouteContext(this, null, function, httpMethod));
                    }
                }

                // Apply extras to routes
                foreach (var attribute in function.GetCustomAttributes()) {
                    switch (attribute) {
                        case RequireSessionAttribute requireSession: {
                            foreach (var routeContext in routeContexts) {
                                routeContext.SetRequireSession(requireSession.RequireSession);
                            }

                            break;
                        }
                        case RequireContentTypeAttribute requireContentType: {
                            foreach (var routeContext in routeContexts) {
                                routeContext.SetRequiredContentType(requireContentType.ContentType);
                            }

                            break;
                        }
                    }
                }

                foreach (var routeContext in routeContexts) {
                    Logger.LogInfo($"Registering route: [{routeContext.Method}] {routeContext.Path}");
                    var route = $"{_prefix}{routeContext.Path}";
                    if (_routes.TryGetValue(route, out List<RouteContext> listRouteContexts)) {
                        listRouteContexts.Add(routeContext);
                    } else {
                        _routes.Add(route, new List<RouteContext> {routeContext});
                    }
                }
            }

            return this;
        }

        public HttpMvc LoadController(object instance) {
            var type = instance.GetType();
            IEnumerable<MethodInfo> functions = type.GetMethods().Where(m => m.GetCustomAttributes(typeof(HttpMethodAttribute), false).Length > 0);
            foreach (var function in functions) {
                if (function.ReturnType != typeof(Task) || function.IsStatic) continue;

                ParameterInfo[] parameters = function.GetParameters();

                if (parameters.Length < 1 || parameters[0].ParameterType != typeof(HtcHttpContext)) continue;

                // Load routes
                var routeContexts = new List<RouteContext>();
                foreach (var attribute in function.GetCustomAttributes()) {
                    if (attribute is HttpMethodAttribute httpMethod) {
                        routeContexts.Add(new RouteContext(this, instance, function, httpMethod));
                    }
                }

                // Apply extras to routes
                foreach (var attribute in function.GetCustomAttributes()) {
                    switch (attribute) {
                        case RequireSessionAttribute requireSession: {
                            foreach (var routeContext in routeContexts) {
                                routeContext.SetRequireSession(requireSession.RequireSession);
                            }

                            break;
                        }
                        case RequireContentTypeAttribute requireContentType: {
                            foreach (var routeContext in routeContexts) {
                                routeContext.SetRequiredContentType(requireContentType.ContentType);
                            }

                            break;
                        }
                    }
                }

                foreach (var routeContext in routeContexts) {
                    Logger.LogInfo($"Registering route: [{routeContext.Method}] {routeContext.Path}");
                    if (_routes.TryGetValue(routeContext.Path, out List<RouteContext> listRouteContexts)) {
                        listRouteContexts.Add(routeContext);
                    } else {
                        _routes.Add(routeContext.Path, new List<RouteContext> {routeContext});
                    }
                }
            }

            return this;
        }

        public HttpMvc MatchDomain(string domain) {
            _domains.Add(domain);
            _matchAnyDomain = _domains.Count == 0;
            return this;
        }

        public HttpMvc RemoveDomain(string domain) {
            _domains.Remove(domain);
            _matchAnyDomain = _domains.Count == 0;
            return this;
        }

        public HttpMvc UsePrefix(string prefix) {
            _prefix = prefix;
            return this;
        }

        public HttpMvc UseParser(Type type, IHttpObjectParser parser) {
            if (!type.IsAssignableFrom(typeof(IHttpObject))) throw new MvcContructException($"Type '{type.FullName}' is not assignable from IHttpObject.");
            if (!type.IsInterface) throw new MvcContructException($"Type '{type.FullName}' should be an interface.");
            if (_parsers.ContainsKey(type)) throw new MvcContructException($"Type '{type.FullName}' already is registered.");
            _parsers.Add(type, parser);
            return this;
        }

        public HttpMvc RemoveParser(Type type) {
            _parsers.Remove(type);
            return this;
        }

        protected virtual Task LoadSession(HtcHttpContext httpContext) => Task.CompletedTask;

        protected virtual async Task ThrowInvalidSession(HtcHttpContext httpContext) {
            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsync("Error: Invalid session.");
        }

        protected virtual async Task ThrowInvalidContentType(HtcHttpContext httpContext) {
            httpContext.Response.StatusCode = 415;
            await httpContext.Response.WriteAsync("Error: Content-Type is invalid or not recognized.");
        }

        protected virtual async Task ThrowHttpException(HtcHttpContext httpContext, HttpException httpException) {
            httpContext.Response.StatusCode = httpException.Status;
            await httpContext.Response.WriteAsync(httpException.Message);
        }

        protected virtual async Task ThrowException(HtcHttpContext httpContext, Exception exception) {
            Logger.LogError($"[{httpContext.Connection.Id}]", exception);
            if (!httpContext.Response.HasStarted) {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync($"[{httpContext.Connection.Id}] An internal failure occurred. Please try again later.");
            }
        }

        protected virtual Task ThrowPreProcessingException(HtcHttpContext httpContext, Exception exception) {
            Logger.LogError(exception);
            return Task.CompletedTask;
        }

        protected virtual async Task ThrowFailedDecodeData(HtcHttpContext httpContext, JsonException exception) {
            if (!httpContext.Response.HasStarted) {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync("Failed to decode request data.");
            }
        }

        internal bool Match(HtcHttpContext httpContext, string path) {
            if (_matchAnyDomain) {
                return _routes.ContainsKey(path);
            } else {
                return _domains.Contains(httpContext.Request.Host.Value) && _routes.ContainsKey(path);
            }
        }

        internal async Task OnHttpRequest(HtcHttpContext httpContext, string path) {
            try {
                if (_routes.TryGetValue(path, out List<RouteContext> routeContexts)) {
                    foreach (var routeContext in routeContexts.Where(context => context.Method == httpContext.Request.Method)) {
                        try {
                            if (routeContext.RequireSession) {
                                if (httpContext.Session == null) await LoadSession(httpContext);
                                if (httpContext.Session == null) {
                                    await ThrowInvalidSession(httpContext);
                                    break;
                                }
                            }

                            if (routeContext.RequiredContentType != null) {
                                string contentType = httpContext.Request.ContentType.Split(";", 2)[0];
                                if (contentType != routeContext.RequiredContentType) {
                                    await ThrowInvalidContentType(httpContext);
                                    break;
                                }
                            }

                            await routeContext.ProcessRequest(httpContext);
                        } catch (HttpException ex) {
                            await ThrowHttpException(httpContext, ex);
                        } catch (JsonException ex) {
                            await ThrowFailedDecodeData(httpContext, ex);
                        } catch (Exception ex) {
                            await ThrowException(httpContext, ex);
                        }

                        break;
                    }
                }
            } catch (Exception ex) {
                await ThrowPreProcessingException(httpContext, ex);
            }
        }
    }
}