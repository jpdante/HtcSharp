using System.Threading.Tasks;

namespace HtcSharp.Abstractions {
    public interface IServer {

        public abstract Task OnReload();

    }
}