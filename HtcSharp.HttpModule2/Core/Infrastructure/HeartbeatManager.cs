using System;
using System.Threading;

namespace HtcSharp.HttpModule2.Core.Infrastructure {
    internal class HeartbeatManager : IHeartbeatHandler, ISystemClock {
        private readonly ConnectionManager _connectionManager;
        private readonly Action<HtcConnection> _walkCallback;
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

        private void WalkCallback(HtcConnection connection) {
            connection.TickHeartbeat();
        }
    }
}