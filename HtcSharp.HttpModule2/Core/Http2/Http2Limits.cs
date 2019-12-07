using System;

namespace HtcSharp.HttpModule2.Core.Http2 {
    public class Http2Limits {
        private int _headerTableSize = (int) Http2PeerSettings.DefaultHeaderTableSize;
        private int _initialConnectionWindowSize = 1024 * 128;
        private int _initialStreamWindowSize = 1024 * 96;
        private int _maxFrameSize = (int) Http2PeerSettings.DefaultMaxFrameSize;
        private int _maxRequestHeaderFieldSize = (int) Http2PeerSettings.DefaultMaxFrameSize;
        private int _maxStreamsPerConnection = 100;

        public int MaxStreamsPerConnection {
            get => _maxStreamsPerConnection;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        "A value greater than zero is required.");
                _maxStreamsPerConnection = value;
            }
        }

        public int HeaderTableSize {
            get => _headerTableSize;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        "A value greater than zero is required.");

                _headerTableSize = value;
            }
        }

        public int MaxFrameSize {
            get => _maxFrameSize;
            set {
                if (value < Http2PeerSettings.MinAllowedMaxFrameSize || value > Http2PeerSettings.MaxAllowedMaxFrameSize
                )
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        $"Argument out of range [{Http2PeerSettings.MinAllowedMaxFrameSize} >= value <= {Http2PeerSettings.MaxAllowedMaxFrameSize}].");

                _maxFrameSize = value;
            }
        }

        public int MaxRequestHeaderFieldSize {
            get => _maxRequestHeaderFieldSize;
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        "A value greater than zero is required.");

                _maxRequestHeaderFieldSize = value;
            }
        }

        public int InitialConnectionWindowSize {
            get => _initialConnectionWindowSize;
            set {
                if (value < Http2PeerSettings.DefaultInitialWindowSize || value > Http2PeerSettings.MaxWindowSize)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        $"Argument out of range [{Http2PeerSettings.DefaultInitialWindowSize} >= value <= {Http2PeerSettings.MaxWindowSize}].");

                _initialConnectionWindowSize = value;
            }
        }

        public int InitialStreamWindowSize {
            get => _initialStreamWindowSize;
            set {
                if (value < Http2PeerSettings.DefaultInitialWindowSize || value > Http2PeerSettings.MaxWindowSize)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        $"Argument out of range [{Http2PeerSettings.DefaultInitialWindowSize} >= value <= {Http2PeerSettings.MaxWindowSize}].");

                _initialStreamWindowSize = value;
            }
        }
    }
}