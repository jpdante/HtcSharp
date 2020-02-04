using System;

namespace HtcSharp.HttpModule.Infrastructure.Heart {
    public class MinDataRate {
        /// <summary>
        /// Creates a new instance of <see cref="MinDataRate"/>.
        /// </summary>
        /// <param name="bytesPerSecond">The minimum rate in bytes/second at which data should be processed.</param>
        /// <param name="gracePeriod">The amount of time to delay enforcement of <paramref name="bytesPerSecond"/>,
        /// starting at the time data is first read or written.</param>
        public MinDataRate(double bytesPerSecond, TimeSpan gracePeriod) {
            if (bytesPerSecond <= 0) {
                throw new ArgumentOutOfRangeException(nameof(bytesPerSecond), "Value must be a positive number. To disable a minimum data rate, use null where a MinDataRate instance is expected.");
            }

            if (gracePeriod <= Heartbeat.Interval) {
                throw new ArgumentOutOfRangeException(nameof(gracePeriod), $"The request body rate enforcement grace period must be greater than {Heartbeat.Interval.TotalSeconds} second.");
            }

            BytesPerSecond = bytesPerSecond;
            GracePeriod = gracePeriod;
        }

        /// <summary>
        /// The minimum rate in bytes/second at which data should be processed.
        /// </summary>
        public double BytesPerSecond { get; }

        /// <summary>
        /// The amount of time to delay enforcement of <see cref="MinDataRate" />,
        /// starting at the time data is first read or written.
        /// </summary>
        public TimeSpan GracePeriod { get; }
    }
}
