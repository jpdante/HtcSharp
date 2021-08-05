namespace HtcSharp.Abstractions {
    public interface IVersion {
        
        public int Major { get; }

        public int Minor { get; }

        public int Patch { get; }

    }
}