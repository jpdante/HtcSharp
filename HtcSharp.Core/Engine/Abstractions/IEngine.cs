using System.Threading.Tasks;
using HtcSharp.Core.Logging.Abstractions;
using Newtonsoft.Json.Linq;

namespace HtcSharp.Core.Engine.Abstractions {
    public interface IEngine {

        static string Name { get; }

        Task Load(JObject config, ILogger logger);
        Task Start();
        Task Stop();

    }
}
