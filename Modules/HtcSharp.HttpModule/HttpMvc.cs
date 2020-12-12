using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions;
using HtcSharp.HttpModule.Http.Abstractions.Extensions;
using HtcSharp.HttpModule.Mvc;
using HtcSharp.HttpModule.Routing;

namespace HtcSharp.HttpModule {
    // SourceTools-Start
    // Ignore-Copyright
    // SourceTools-End
    public abstract class HttpMvc : IHttpEvents {

        private readonly Dictionary<string, (HttpMethodAttribute, bool, Type, MethodInfo)> _routes;

        private ILogger _logger;
        private Assembly _assembly;

        protected HttpMvc() {
            _routes = new Dictionary<string, (HttpMethodAttribute, bool, Type, MethodInfo)>();
        }

        public void Setup(Assembly assembly, ILogger logger) {
            _logger = logger;
            _assembly = assembly;
            ReloadControllers();
        }

        public void ReloadControllers() {
            _routes.Clear();
            MethodInfo[] methods = _assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(HttpMethodAttribute), false).Length > 0)
                .ToArray();
            foreach (var method in methods) {
                if (method.ReturnType != typeof(Task) || !method.IsStatic) continue;
                ParameterInfo[] parameters = method.GetParameters();
                var attribute = method.GetCustomAttributes(typeof(HttpMethodAttribute), false).First() as HttpMethodAttribute;
                if (attribute == null) continue;
                if (parameters.Length < 1 || parameters[0].ParameterType != typeof(HttpContext)) continue;
                if (parameters.Length == 2 && !parameters[1].ParameterType.GetInterfaces().Contains(typeof(IHttpJsonObject))) continue;
                _logger?.LogInfo(parameters.Length == 2 ? $"Registering route: [{attribute.Method}, JsonObject] {attribute.Path}" : $"Registering route: [{attribute.Method}] {attribute.Path}");
                _routes.Add($"{attribute.Method.ToUpper()}{attribute.Path}", (attribute, parameters.Length == 2, parameters.Length == 2 ? parameters[1].ParameterType : null, method));
                if (UrlMapper.RegisteredPages.TryGetValue(attribute.Path, out var httpEvents)) {
                    if (httpEvents != this) {
                        throw new Exception($"The '{attribute.Path}' page has already been registered by another plugin.");
                    }
                    return;
                }
                UrlMapper.RegisterPluginPage(attribute.Path, this);
            }
        }

        public virtual Task LoadSession(HttpContext httpContext) {
            return Task.CompletedTask;
        }

        public virtual async Task ThrowInvalidSession(HttpContext httpContext) {
            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsync("Invalid session.");
        }

        public virtual async Task ThrowInvalidContentType(HttpContext httpContext) {
            httpContext.Response.StatusCode = 415;
            await httpContext.Response.WriteAsync("Content-Type invalid or not recognized.");
        }

        public virtual async Task ThrowHttpException(HttpContext httpContext, HttpException httpException) {
            httpContext.Response.StatusCode = httpException.Status;
            await httpContext.Response.WriteAsync(httpException.Message);
        }

        public virtual async Task ThrowException(HttpContext httpContext, Exception exception) {
            _logger?.LogError($"[{httpContext.Connection.Id}]", exception);
            _logger?.LogError(exception);
            if (!httpContext.Response.HasStarted) {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync($"[{httpContext.Connection.Id}] An internal failure occurred. Please try again later.");
            }
        }

        public virtual Task ThrowPreProcessingException(HttpContext httpContext, Exception exception) {
            _logger?.LogError(exception);
            return Task.CompletedTask;
        }

        public virtual async Task ThrowFailedDecodeData(HttpContext httpContext, JsonException exception) {
            if (!httpContext.Response.HasStarted) {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync("Failed to decode request data.");
            }
        }

        public virtual Task<bool> BeforePageRequest(HttpContext httpContext, string filename) {
            return Task.FromResult(false);
        }

        public async Task OnHttpPageRequest(HttpContext httpContext, string filename) {
            if (await BeforePageRequest(httpContext, filename)) return;
            try {
                if (_routes.TryGetValue($"{httpContext.Request.Method.ToUpper()}{filename}", out var value)) {
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
                                    data = new[] {httpContext, obj};
                                else {
                                    await ThrowFailedDecodeData(httpContext, null);
                                    return;
                                }
                                /*using var streamReader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
                                if (JsonConvertExt.TryDeserializeObject(await streamReader.ReadToEndAsync(), value.Item3, out var obj)) {
                                    if (obj is IHttpJsonObject httpJsonObject && await httpJsonObject.ValidateData(httpContext))
                                        data = new object[] { httpContext, obj };
                                    else {
                                        if (!httpContext.Response.HasStarted) await httpContext.Response.SendDecodeErrorAsync();
                                        return;
                                    }
                                } else {
                                    await httpContext.Response.SendDecodeErrorAsync();
                                }*/

                                //streamReader.Close();
                            } else {
                                data = new object[] {httpContext};
                            }

                            // ReSharper disable once PossibleNullReferenceException
                            await (Task) value.Item4.Invoke(null, data);
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
