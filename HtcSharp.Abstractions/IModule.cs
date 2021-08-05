using System;
using System.Threading.Tasks;

namespace HtcSharp.Abstractions {
    public interface IModule : IReadOnlyModule, IDisposable {

        Task Init(IServiceProvider serviceProvider);
        Task Enable();
        Task Disable();

    }
}