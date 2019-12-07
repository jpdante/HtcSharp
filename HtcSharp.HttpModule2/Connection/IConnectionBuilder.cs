using System;

namespace HtcSharp.HttpModule2.Connection {
    public interface IConnectionBuilder {

        IServiceProvider ApplicationServices { get; }

        IConnectionBuilder Use(Func<ConnectionDelegate, ConnectionDelegate> middleware);

        ConnectionDelegate Build();
    }
}