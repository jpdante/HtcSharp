using System;
using System.Collections.Generic;
using System.Text;
using HtcSharp.Core.Models.Http;

namespace HtcSharp.Core.Interfaces.Http {
    public interface IDirective {

        void Execute(HtcHttpContext context);

    }
}
