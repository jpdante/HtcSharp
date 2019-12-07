using System;
using System.Collections.Generic;

namespace HtcSharp.HttpModule2.Connection {
    public interface IFeatureCollection : IEnumerable<KeyValuePair<Type, object>> {
        bool IsReadOnly { get; }

        int Revision { get; }

        object this[Type key] { get; set; }

        TFeature Get<TFeature>();

        void Set<TFeature>(TFeature instance);
    }
}
