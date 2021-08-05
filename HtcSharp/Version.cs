using HtcSharp.Abstractions;

namespace HtcSharp {
    public class Version : IVersion {

        public int Major { get; init; }

        public int Minor { get; init; }

        public int Patch { get; init; }

    }
}