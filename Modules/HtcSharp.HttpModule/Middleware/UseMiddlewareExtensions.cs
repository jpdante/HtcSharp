using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using HtcSharp.HttpModule.Abstractions;
using HtcSharp.HttpModule.Http;
using Microsoft.Extensions.DependencyInjection;
using IMiddleware = HtcSharp.HttpModule.Abstractions.IMiddleware;

namespace HtcSharp.HttpModule.Middleware {
    public static class UseMiddlewareExtensions {
        internal const string InvokeMethodName = "Invoke";
        internal const string InvokeAsyncMethodName = "InvokeAsync";

        private static readonly MethodInfo GetServiceInfo = typeof(UseMiddlewareExtensions).GetMethod(nameof(GetService), BindingFlags.NonPublic | BindingFlags.Static)!;

        private const DynamicallyAccessedMemberTypes MiddlewareAccessibility = DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods;

        public static IMiddlewareBuilder UseMiddleware<[DynamicallyAccessedMembers(MiddlewareAccessibility)]TMiddleware>(this IMiddlewareBuilder builder, params object[] args) {
            return builder.UseMiddleware(typeof(TMiddleware), args);
        }

        public static IMiddlewareBuilder UseMiddleware(this IMiddlewareBuilder builder, [DynamicallyAccessedMembers(MiddlewareAccessibility)] Type middleware, params object[] args) {
            var applicationServices = builder.ApplicationServices;
            return builder.Use(next => {
                var methods = middleware.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                var invokeMethods = methods.Where(m =>
                    string.Equals(m.Name, InvokeMethodName, StringComparison.Ordinal)
                    || string.Equals(m.Name, InvokeAsyncMethodName, StringComparison.Ordinal)
                    ).ToArray();

                if (invokeMethods.Length > 1) {
                    throw new InvalidOperationException($"Multiple public '{InvokeMethodName}' or '{InvokeAsyncMethodName}' methods are available.");
                }

                if (invokeMethods.Length == 0) {
                    throw new InvalidOperationException($"No public '{InvokeMethodName}' or '{InvokeAsyncMethodName}' method found for middleware of type '{middleware}'.");
                }

                var methodInfo = invokeMethods[0];
                if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType)) {
                    throw new InvalidOperationException($"'{InvokeMethodName}' or '{InvokeAsyncMethodName}' does not return an object of type '{nameof(Task)}'.");
                }

                var parameters = methodInfo.GetParameters();
                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(HtcHttpContext)) {
                    throw new InvalidOperationException($"The '{InvokeMethodName}' or '{InvokeAsyncMethodName}' method's first argument must be of type '{nameof(HtcHttpContext)}'.");
                }

                var ctorArgs = new object[args.Length + 1];
                ctorArgs[0] = next;
                Array.Copy(args, 0, ctorArgs, 1, args.Length);
                var instance = ActivatorUtilities.CreateInstance(builder.ApplicationServices, middleware, ctorArgs);
                if (parameters.Length == 1) {
                    return (RequestDelegate)methodInfo.CreateDelegate(typeof(RequestDelegate), instance);
                }

                var factory = Compile<object>(methodInfo, parameters);

                return context => {
                    var serviceProvider = context.RequestServices ?? applicationServices;
                    if (serviceProvider == null) {
                        throw new InvalidOperationException($"'{nameof(IServiceProvider)}' is not available.");
                    }

                    return factory(instance, context, serviceProvider);
                };
            });
        }

        private static Func<T, HtcHttpContext, IServiceProvider, Task> Compile<T>(MethodInfo methodInfo, ParameterInfo[] parameters) {
            var middleware = typeof(T);

            var httpContextArg = Expression.Parameter(typeof(HtcHttpContext), "httpContext");
            var providerArg = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
            var instanceArg = Expression.Parameter(middleware, "middleware");

            var methodArguments = new Expression[parameters.Length];
            methodArguments[0] = httpContextArg;
            for (var i = 1; i < parameters.Length; i++) {
                var parameterType = parameters[i].ParameterType;
                if (parameterType.IsByRef) {
                    throw new NotSupportedException($"The '{InvokeMethodName}' method must not have ref or out parameters.");
                }

                var parameterTypeExpression = new Expression[]
                {
                    providerArg,
                    Expression.Constant(parameterType, typeof(Type)),
                    Expression.Constant(methodInfo.DeclaringType, typeof(Type))
                };

                var getServiceCall = Expression.Call(GetServiceInfo, parameterTypeExpression);
                methodArguments[i] = Expression.Convert(getServiceCall, parameterType);
            }

            Expression middlewareInstanceArg = instanceArg;
            if (methodInfo.DeclaringType != null && methodInfo.DeclaringType != typeof(T)) {
                middlewareInstanceArg = Expression.Convert(middlewareInstanceArg, methodInfo.DeclaringType);
            }

            var body = Expression.Call(middlewareInstanceArg, methodInfo, methodArguments);

            var lambda = Expression.Lambda<Func<T, HtcHttpContext, IServiceProvider, Task>>(body, instanceArg, httpContextArg, providerArg);

            return lambda.Compile();
        }

        private static object GetService(IServiceProvider sp, Type type, Type middleware) {
            var service = sp.GetService(type);
            if (service == null) {
                throw new InvalidOperationException($"Unable to resolve service for type '{type}' while attempting to Invoke middleware '{middleware}'.");
            }

            return service;
        }
    }
}