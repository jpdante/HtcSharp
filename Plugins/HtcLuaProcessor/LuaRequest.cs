using System;
using HtcSharp.Core.Models.Http;

namespace HtcLuaProcessor {
    public class LuaRequest {
        public LuaRequest(string filename, HtcHttpContext htpContext) {
            
        }

        public bool DoProcess() {
            return false;
        }

        public void RegisterLuaCommand(string name, Action action) {
            
        }

        public void RegisterObject(string name, object data) {
            
        }
        
        
    }
}