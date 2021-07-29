using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.Logging;
using Microsoft.AspNetCore.Http;

namespace HtcSharp.HttpModule.Mvc {
    public abstract class HttpMvc {

        private readonly ILogger Logger = LoggerManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly Dictionary<string, (HttpMethodAttribute, bool, Type, MethodInfo)> _routes;

        private Assembly _assembly;

        protected HttpMvc() {
            _routes = new Dictionary<string, (HttpMethodAttribute, bool, Type, MethodInfo)>();
        }

        public void Setup(Assembly assembly) {
            _assembly = assembly;
            ReloadControllers();
        }

        public void ReloadControllers() {
            _routes.Clear();
            MethodInfo[] methods = _assembly.GetTypes().SelectMany(t => t.GetMethods()).Where(m => m.GetCustomAttributes(typeof(HttpMethodAttribute), false).Length > 0).ToArray();
            foreach (var method in methods) {
                if (method.ReturnType != typeof(Task) || !method.IsStatic) continue;
                ParameterInfo[] parameters = method.GetParameters();
                var attribute = method.GetCustomAttributes(typeof(HttpMethodAttribute), false).First() as HttpMethodAttribute;
                if (attribute == null) continue;
                if (parameters.Length < 1 || parameters[0].ParameterType != typeof(HttpContext)) continue;
                if (parameters.Length == 2 && !parameters[1].ParameterType.GetInterfaces().Contains(typeof(IHttpJsonObject))) continue;
                Logger.LogInfo(parameters.Length == 2 ? $"Registering route: [{attribute.Method}, JsonObject] {attribute.Path}" : $"Registering route: [{attribute.Method}] {attribute.Path}");
                if (_routes.ContainsKey($"{attribute.Method.ToUpper()}{attribute.Path}")) throw new DuplicateNameException($"Duplicate route [{attribute.Method.ToUpper()}] {attribute.Path}");
                _routes.Add($"{attribute.Method.ToUpper()}{attribute.Path}", (attribute, parameters.Length == 2, parameters.Length == 2 ? parameters[1].ParameterType : null, method));
                if (!UrlMapper.RegisteredPages.ContainsKey(attribute.Path)) UrlMapper.RegisterPluginPage(attribute.Path, this);
            }
        }

        protected virtual Task LoadSession(HttpContext httpContext) {
            return Task.CompletedTask;
        }

        protected virtual async Task ThrowInvalidSession(HttpContext httpContext) {
            httpContext.Response.StatusCode = 403;
            await httpContext.Response.WriteAsync("Invalid session.");
        }

        protected virtual async Task ThrowInvalidContentType(HttpContext httpContext) {
            httpContext.Response.StatusCode = 415;
            await httpContext.Response.WriteAsync("Content-Type invalid or not recognized.");
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