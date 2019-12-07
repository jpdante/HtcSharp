using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using HtcSharp.HttpModule2.Connection.Address;

namespace HtcSharp.HttpModule2.Connection.ListenOptions {
    internal sealed class AnyIPListenOptions : ListenOptions {
        internal AnyIPListenOptions(int port) : base(new IPEndPoint(IPAddress.IPv6Any, port)) {
        }

        internal override async Task BindAsync(AddressBindContext context) {
            try {
                await base.BindAsync(context).ConfigureAwait(false);
            } catch (Exception ex) when (!(ex is IOException)) {
                context.Logger.LogDebug(CoreStrings.FormatFallbackToIPv4Any(IPEndPoint.Port));
                EndPoint = new IPEndPoint(IPAddress.Any, IPEndPoint.Port);
                await base.BindAsync(context).ConfigureAwait(false);
            }
        }
    }
}
