using System.Threading.Tasks;

namespace HtcSharp.Abstractions.Internal {
    public interface IServer {

        public abstract Task OnReload();

    }
}