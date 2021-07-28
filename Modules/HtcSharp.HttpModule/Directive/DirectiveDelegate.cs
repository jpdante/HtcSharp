using System.Threading.Tasks;
using HtcSharp.HttpModule.Http;

namespace HtcSharp.HttpModule.Directive {

    public delegate Task DirectiveDelegate(HtcHttpContext context);

}