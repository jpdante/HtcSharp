using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCSharp.Core.Components.Http {
    public class HttpEngineContainer : ServiceDescriptor {
        public HttpEngineContainer(Type serviceType, Type implementationType, ServiceLifetime lifetime) : base(serviceType, implementationType, lifetime) {
        }
    }
}
