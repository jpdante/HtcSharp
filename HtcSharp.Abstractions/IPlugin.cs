using System;
using System.Threading.Tasks;

namespace HtcSharp.Abstractions {
    public interface IPlugin : IReadOnlyPlugin, IDisposable {

        Task Init(IServiceProvider serviceProvider);
        Task Enable();
        Task Disable();

    }
}