using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions.Mvc;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Mvc.Exceptions;

namespace HtcSharp.HttpModule.Mvc {
    public class RouteContext {

        private readonly object _instance;
        private readonly MethodInfo _methodInfo;
        private readonly Type _parameterType;
        private readonly List<IHttpObjectParser> _parsers;

        public string Path { get; private set; }
        public string Method { get; private set; }
        public bool RequireObject { get; private set; }
        public bool RequireSession { get; private set; }
        public bool ReturnObject { get; private set; }
        public string RequiredContentType { get; private set; }

        public RouteContext(HttpMvc httpMvc, object instance, MethodInfo methodInfo, HttpMethodAttribute httpMethod) {
            _instance = instance;
            _methodInfo = methodInfo;
            _parameterType = null;
            _parsers = new List<IHttpObjectParser>();

            Path = httpMethod.Path;
            Method = httpMethod.Method;
            RequireObject = false;
            RequireSession = false;
            ReturnObject = _methodInfo.ReturnType == typeof(Task<>);
            RequiredContentType = null;

            var parameters = _methodInfo.GetParameters();
            switch (parameters.Length) {
                case 1:
                    break;
                case 2: {
                    RequireObject = true;
                    var objectParameter = parameters[1];
                    _parameterType = objectParameter.ParameterType;
                    foreach (var (type, parser) in httpMvc.Parsers) {
                        if (!type.IsAssignableFrom(_parameterType)) continue;
                        _parsers.Add(parser);
                    }
                    break;
                }
                default:
                    throw new MvcContructException($"Failed to load controller function '{_methodInfo.Name}' using 'HTTP [{Method}] {httpMvc.Parsers}', too many parameters.");
            }
        }

        public void SetRequireSession(bool value) {
            RequireSession = value;
        }

        public void SetRequiredContentType(string contentType) {
            RequiredContentType = contentType;
        }

        public async Task ProcessRequest(HtcHttpContext httpContext) {
            object[] parameterData;
            if (RequireObject) {
                var exceptionList = new List<Exception>();
                IHttpObject parsedHttpObject = null;
                foreach (var parser in _parsers) {
                    try {
                        parsedHttpObject = await parser.Parse(httpContext, _parameterType);
                    } catch (Exception ex) {
                        exceptionList.Add(ex);
                    }
                    if (parsedHttpObject == null) continue;
                }
                if (parsedHttpObject == null) throw new HttpDecodeDataException(exceptionList.ToArray());
                if (!await parsedHttpObject.ValidateData(httpContext)) throw new HttpDataValidationException();
                parameterData = new object[] { httpContext, parsedHttpObject };
            } else {
                parameterData = new object[] { httpContext };
            }
            var invokeReturn = _methodInfo.Invoke(_instance, parameterData);
            if (invokeReturn is Task<object> taskReturn) {
                var data = await taskReturn;
                try {
                    await JsonSerializer.SerializeAsync(httpContext.Response.Body, data);
                } catch (Exception ex) {
                    throw new HttpEncodeDataException(ex);
                }
            } else if (invokeReturn is Task task) {
                await task;
            } else throw new HttpNullTaskException();
        }
    }
}