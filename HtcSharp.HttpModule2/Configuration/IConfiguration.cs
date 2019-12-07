using System.Collections.Generic;

namespace HtcSharp.HttpModule2.Configuration {
    public interface IConfiguration {

        string this[string key] { get; set; }

        IConfigurationSection GetSection(string key);

        IEnumerable<IConfigurationSection> GetChildren();

        IChangeToken GetReloadToken();
    }
}