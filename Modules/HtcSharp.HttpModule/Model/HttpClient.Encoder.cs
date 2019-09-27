namespace HtcSharp.HttpModule.Model {
    public partial class HttpClient {
        private class Encoder {
            private HttpClient _owner;

            public Encoder(HttpClient owner) {
                _owner = owner;
            }

            public async void RunAsync() { }
        }
    }
}
