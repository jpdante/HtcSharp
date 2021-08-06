using System;
using System.Threading.Tasks;
using HtcSharp.Abstractions.Manager;

namespace HtcSharp.Abstractions {
    public interface IPlugin : IReadOnlyPlugin, IDisposable {

        Task Init(IServiceProvider serviceProvider);
        Task Enable();
        Task Disable();

    }
}