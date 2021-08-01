using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Http;

namespace HtcSharp.HttpModule.Mvc {
    public abstract class HttpMvc {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private Assembly _assembly;

        private readonly Dictionary<string, List<RouteContext>> _routes;

        protected HttpMvc() {
            _assembly = null;
        }

        public void Setup(Assembly assembly) {
            _assembly = assembly;
            ReloadControllers();
        }

        public void ReloadControllers() {
            _routes.Clear();
            IEnumerable<MethodInfo> functions = _assembly.GetTypes().SelectMany(t => t.GetMethods()).Where(m => m.GetCustomAttributes(typeof(HttpMethodAttribute), false).Length > 0);
            foreach (var function in functions) {
                if (function.ReturnType != typeof(Task) || !function.IsStatic) continue;
                ParameterInfo[] parameters = function.GetParameters();

                // Load routes
                var routeContexts = new List<RouteContext>();
                foreach (var attribute in function.GetCustomAttributes()) {
                    if (attribute is HttpMethodAttribute httpMethod) {
                        routeContexts.Add(new RouteContext(httpMethod));
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
                if (parameters.Length < 1 || parameters[0].ParameterType != typeof(HtcHttpContext)) continue;
                foreach (var routeContext in routeContexts) {
                    if (_routes.TryGetValue(routeContext.Path, out List<RouteContext> listRouteContexts)) {
                        listRouteContexts.Add(routeContext);
                    } else {
                        _routes.Add(routeContext.Path, new List<RouteContext> { routeContext });
                    }
                }
                /*
                if (parameters.Length == 2 && !parameters[1].ParameterType.GetInterfaces().Contains(typeof(IHttpJsonObject))) continue;
                Logger.LogInfo(parameters.Length == 2 ? $"Registering route: [{attribute.Method}, JsonObject] {attribute.Path}" : $"Registering route: [{attribute.Method}] {attribute.Path}");
                if (_routes.ContainsKey($"{attribute.Method.ToUpper()}{attribute.Path}")) throw new DuplicateNameException($"Duplicate route [{attribute.Method.ToUpper()}] {attribute.Path}");
                _routes.Add($"{attribute.Method.ToUpper()}{attribute.Path}", (attribute, parameters.Length == 2, parameters.Length == 2 ? parameters[1].ParameterType : null, method));
                PathList.Add(attribute.Path);
                if (!UrlMapper.RegisteredPages.ContainsKey(attribute.Path)) UrlMapper.RegisterPluginPage(attribute.Path, this);*/
            }
        }

        protected virtual Task LoadSession(HttpContext httpContext) {
            return Task.CompletedTask;
        }

        protected virtual async Task ThrowInvalidSession(HttpContext httpContext) {
            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsync("Error: Invalid session.");
        }

        protected virtual async Task ThrowInvalidContentType(HttpContext httpContext) {
            httpContext.Response.StatusCode = 415;
            await httpContext.Response.WriteAsync("Error: Content-Type is invalid or not recognized.");
        }

        protected virtual async Task ThrowHttpException(HttpContext httpContext, HttpException httpException) {
            httpContext.Response.StatusCode = httpException.Status;
            await httpContext.Response.WriteAsync(httpException.Message);
        }

        protected virtual async Task ThrowException(HttpContext httpContext, Exception exception) {
            Logger.LogError($"[{httpContext.Connection.Id}]", exception);
            Logger.LogError(exception);
            if (!httpContext.Response.HasStarted) {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync($"[{httpContext.Connection.Id}] An internal failure occurred. Please try again later.");
            }
        }

        protected virtual Task ThrowPreProcessingException(HttpContext httpContext, Exception exception) {
            Logger.LogError(exception);
            return Task.CompletedTask;
        }

        protected virtual async Task ThrowFailedDecodeData(HttpContext httpContext, JsonException exception) {
            if (!httpContext.Response.HasStarted) {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync("Failed to decode request data.");
            }
        }

        protected virtual Task<bool> BeforePageRequest(HttpContext httpContext, string filename) {
            return Task.FromResult(false);
        }

        public async Task OnHttpRequest(HttpContext httpContext, string fileName) {
            if (await BeforePageRequest(httpContext, fileName)) return;
            try {
                if (_routes.TryGetValue($"{httpContext.Request.Method.ToUpper()}{fileName}", out var value)) {
                    await LoadSession(httpContext);
                    if (value.Item1.RequireSession && httpContext.Session != null && !httpContext.Session.IsAvailable) {
                        await ThrowInvalidSession(httpContext);
                        return;
                    }
                    if (value.Item1.RequireContentType != null) {
                        if (httpContext.Request.ContentType == null || httpContext.Request.ContentType.Split(";")[0] != value.Item1.RequireContentType) {
                            await ThrowInvalidContentType(httpContext);
                            return;
                        }
                    }
                    try {
                        object[] data;
                        if (value.Item2) {
                            var obj = await JsonSerializer.DeserializeAsync(httpContext.Request.Body, value.Item3);
                            if (obj != null && obj is IHttpJsonObject httpJsonObject && await httpJsonObject.ValidateData(httpContext))
                                data = new[] { httpContext, obj };
                            else {
                                await ThrowFailedDecodeData(httpContext, null);
                                return;
                            }
                        } else {
                            data = new object[] { httpContext };
                        }
                        // ReSharper disable once PossibleNullReferenceException
                        await (Task)value.Item4.Invoke(null, data);
                    } catch (HttpException ex) {
                        await ThrowHttpException(httpContext, ex);
                    } catch (JsonException ex) {
                        await ThrowFailedDecodeData(httpContext, ex);
                    } catch (Exception ex) {
                        await ThrowException(httpContext, ex);
                    }
                }
            } catch (Exception ex) {
                await ThrowPreProcessingException(httpContext, ex);
            }
        }

        public virtual Task OnHttpExtensionRequest(HttpContext httpContext, string filename, string extension) {
            return Task.CompletedTask;
        }
    }
}