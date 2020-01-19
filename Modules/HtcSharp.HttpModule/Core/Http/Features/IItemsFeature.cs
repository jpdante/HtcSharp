using System.Collections.Generic;

namespace HtcSharp.HttpModule.Core.Http.Features {
    public interface IItemsFeature {
        IDictionary<object, object> Items { get; set; }
    }
}