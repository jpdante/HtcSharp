using System;
using System.Threading;
using HtcSharp.HttpModule.Core.Infrastructure;
using HtcSharp.HttpModule.Infrastructure.Interfaces;

namespace HtcSharp.HttpModule.Infrastructure.Heartbeat {
    internal class HeartbeatManager : IHeartbeatHandler, ISystemClock {
        private readonly ConnectionManager _connectionManager;
        private readonly Action<KestrelConnection> _walkCallback;
        private DateTimeOffset _now;
        private long _nowTicks;

        public HeartbeatManager(ConnectionManager connectionManager) {
            _connectionManager = connectionManager;
            _walkCallback = WalkCallback;
        }

        public DateTimeOffset UtcNow => new DateTimeOffset(UtcNowTicks, TimeSpan.Zero);

        public long UtcNowTicks => Volatile.Read(ref _nowTicks);

        public DateTimeOffset UtcNowUnsynchronized => _now;

        public void OnHeartbeat(DateTimeOffset now) {
            _now = now;
            Volatile.Write(ref _nowTicks, now.Ticks);

            _connectionManager.Walk(_walkCallback);
        }

        private void WalkCallback(KestrelConnection connection) {
            connection.TickHeartbeat();
        }
    }
}