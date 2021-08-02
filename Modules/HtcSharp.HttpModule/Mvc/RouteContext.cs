using System;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions.Mvc;
using HtcSharp.HttpModule.Http;
using HtcSharp.HttpModule.Mvc.Exceptions;

namespace HtcSharp.HttpModule.Mvc {
    public struct RouteContext {

        private readonly HttpMvc _httpMvc;
        private readonly object _instance;
        private readonly MethodInfo _methodInfo;
        private readonly Type _parameterType;

        public string Path { get; private set; }
        public string Method { get; private set; }
        public bool RequireObject { get; private set; }
        public bool RequireSession { get; private set; }
        public string RequiredContentType { get; private set; }

        public RouteContext(HttpMvc httpMvc, object instance, MethodInfo methodInfo, HttpMethodAttribute httpMethod) {
            _httpMvc = httpMvc;
            _instance = instance;
            _methodInfo = methodInfo;
            _parameterType = null;

            Path = httpMethod.Path;
            Method = httpMethod.Method;
            RequireObject = false;
            RequireSession = false;
            RequiredContentType = null;

            ParameterInfo[] parameters = _methodInfo.GetParameters();
            switch (parameters.Length) {
                case 1:
                    break;
                case 2: {
                    RequireObject = true;
                    var objectParameter = parameters[1];
                    _parameterType = objectParameter.ParameterType;
                    foreach (var a in objectParameter.ParameterType.GetInterfaces()) {
                        Console.WriteLine(a.FullName);   
                    }
                    break;
                }
                default:
                    throw new MvcContructException($"Failed to load controller function '{_methodInfo.Name}' using 'HTTP [{Method}] {_httpMvc.Parsers}', too many parameters.");
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
                IHttpObject parsedHttpObject = null;
                foreach (var (type, parser) in _httpMvc.Parsers) {
                    if (!type.IsAssignableFrom(_parameterType)) continue;
                    try {
                        parsedHttpObject = await parser.Parse(httpContext, _parameterType);
                    } catch (Exception) {
                        // ignored
                    }
                    if (parsedHttpObject == null) continue;
                }
                if (parsedHttpObject == null) throw new HttpException(500, "Failed to decode data.");
                if (!await parsedHttpObject.ValidateData(httpContext)) throw new HttpException(403, "Failed to validate data.");
                parameterData = new object[] { httpContext, parsedHttpObject };
            } else {
                parameterData = new object[] { httpContext };
            }
            object task = _methodInfo.Invoke(_instance, parameterData);
            if (task == null) throw new HttpException(500, "Returned task is null.");
            await (Task) task;
        }
    }
}