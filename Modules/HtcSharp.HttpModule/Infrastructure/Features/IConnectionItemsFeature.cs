using System.Collections.Generic;

namespace HtcSharp.HttpModule.Infrastructure.Features {
    public interface IConnectionItemsFeature {
        IDictionary<object, object> Items { get; set; }
    }
}