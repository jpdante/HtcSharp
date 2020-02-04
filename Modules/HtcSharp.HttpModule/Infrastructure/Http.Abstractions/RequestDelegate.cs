using System.Threading.Tasks;

namespace HtcSharp.HttpModule.Infrastructure.Http.Abstractions {
    /// <summary>
    /// A function that can process an HTTP request.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the request.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public delegate Task RequestDelegate(HttpContext context);
}