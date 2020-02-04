using System.Threading.Tasks;
using HtcSharp.HttpModule.Core;

namespace HtcSharp.HttpModule.Http.Http.Abstractions {
    /// <summary>
    /// A function that can process a connection.
    /// </summary>
    /// <param name="connection">A <see cref="ConnectionContext" /> representing the connection.</param>
    /// <returns>A <see cref="Task"/> that represents the connection lifetime. When the task completes, the connection will be closed.</returns>
    public delegate Task ConnectionDelegate(ConnectionContext connection);
}