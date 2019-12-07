using System;
using System.Threading;
using HtcSharp.HttpModule2.Core.Http2;

namespace HtcSharp.HttpModule2.Core {
    public class HtcServerLimits {
        private long? _maxResponseBufferSize = 64 * 1024;

        private long? _maxRequestBufferSize = 1024 * 1024;

        private int _maxRequestLineSize = 8 * 1024;

        private int _maxRequestHeadersTotalSize = 32 * 1024;

        private long? _maxRequestBodySize = 30000000;

        private int _maxRequestHeaderCount = 100;

        private TimeSpan _keepAliveTimeout = TimeSpan.FromMinutes(2);

        private TimeSpan _requestHeadersTimeout = TimeSpan.FromSeconds(30);

        private long? _maxConcurrentConnections = null;
        private long? _maxConcurrentUpgradedConnections = null;

        public long? MaxResponseBufferSize {
            get => _maxResponseBufferSize;
            set {
                if (value.HasValue && value.Value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), CoreStrings.NonNegativeNumberOrNullRequired);
                }
                _maxResponseBufferSize = value;
            }
        }

        public long? MaxRequestBufferSize {
            get => _maxRequestBufferSize;
            set {
                if (value.HasValue && value.Value <= 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), CoreStrings.PositiveNumberOrNullRequired);
                }
                _maxRequestBufferSize = value;
            }
        }

        public int MaxRequestLineSize {
            get => _maxRequestLineSize;
            set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), CoreStrings.PositiveNumberRequired);
                }
                _maxRequestLineSize = value;
            }
        }

        public int MaxRequestHeadersTotalSize {
            get => _maxRequestHeadersTotalSize;
            set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), CoreStrings.PositiveNumberRequired);
                }
                _maxRequestHeadersTotalSize = value;
            }
        }

        public int MaxRequestHeaderCount {
            get => _maxRequestHeaderCount;
            set {
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), CoreStrings.PositiveNumberRequired);
                }
                _maxRequestHeaderCount = value;
            }
        }

        public long? MaxRequestBodySize {
            get => _maxRequestBodySize;
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), CoreStrings.NonNegativeNumberOrNullRequired);
                }
                _maxRequestBodySize = value;
            }
        }

        public TimeSpan KeepAliveTimeout {
            get => _keepAliveTimeout;
            set {
                if (value <= TimeSpan.Zero && value != Timeout.InfiniteTimeSpan) {
                    throw new ArgumentOutOfRangeException(nameof(value), CoreStrings.PositiveTimeSpanRequired);
                }
                _keepAliveTimeout = value != Timeout.InfiniteTimeSpan ? value : TimeSpan.MaxValue;
            }
        }

        public TimeSpan RequestHeadersTimeout {
            get => _requestHeadersTimeout;
            set {
                if (value <= TimeSpan.Zero && value != Timeout.InfiniteTimeSpan) {
                    throw new ArgumentOutOfRangeException(nameof(value), CoreStrings.PositiveTimeSpanRequired);
                }
                _requestHeadersTimeout = value != Timeout.InfiniteTimeSpan ? value : TimeSpan.MaxValue;
            }
        }

        public long? MaxConcurrentConnections {
            get => _maxConcurrentConnections;
            set {
                if (value.HasValue && value <= 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), CoreStrings.PositiveNumberOrNullRequired);
                }
                _maxConcurrentConnections = value;
            }
        }

        public long? MaxConcurrentUpgradedConnections {
            get => _maxConcurrentUpgradedConnections;
            set {
                if (value.HasValue && value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(value), CoreStrings.NonNegativeNumberOrNullRequired);
                }
                _maxConcurrentUpgradedConnections = value;
            }
        }

        public Http2Limits Http2 { get; } = new Http2Limits();

        public MinDataRate MinRequestBodyDataRate { get; set; } = new MinDataRate(bytesPerSecond: 240, gracePeriod: TimeSpan.FromSeconds(5));

        public MinDataRate MinResponseDataRate { get; set; } = new MinDataRate(bytesPerSecond: 240, gracePeriod: TimeSpan.FromSeconds(5));
    }
}
