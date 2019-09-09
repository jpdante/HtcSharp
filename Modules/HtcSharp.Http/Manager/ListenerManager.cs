using System;
using System.Collections.Generic;
using System.Net;
using HtcSharp.Http.Net;

namespace HtcSharp.Http.Manager {
    public class ListenerManager {
        private readonly List<SocketListener> _listeners;

        public ListenerManager() {
            _listeners = new List<SocketListener>();
        }

        public int CreateListener(IPEndPoint endPoint) {
            var listener = new SocketListener(endPoint);
            _listeners.Add(listener);
            return _listeners.IndexOf(listener);
        }

        public void DeleteListener(int index) {
            if(_listeners[index].IsListening) _listeners[index].UnBind();
            _listeners.RemoveAt(index);
        }

        public void DeleteListener(SocketListener listener) {
            if(listener.IsListening) listener.UnBind();
            _listeners.Remove(listener);
        }

        public SocketListener GetListener(int index) {
            return index <= _listeners.Count - 1 ? _listeners[index] : null;
        }
        public SocketListener[] GetListeners() {
            return _listeners.ToArray();
        }

        public void StartListener(int index, int backlog = 1) {
            _listeners[index].Bind(backlog);
        }

        public void StopListener(int index) {
            _listeners[index].UnBind();
        }

        public void StartAllListeners(int backlog = 1) {
            foreach (var listener in _listeners) {
                listener.Bind(backlog);
            }
        }

        public void StopAllListeners() {
            foreach (var listener in _listeners) {
                listener.UnBind();
            }
        }
    }
}