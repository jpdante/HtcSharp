using System.Collections.Generic;

namespace HtcSharp.HttpModule.Infrastructure.Features {
    public interface IItemsFeature {
        IDictionary<object, object> Items { get; set; }
    }
}