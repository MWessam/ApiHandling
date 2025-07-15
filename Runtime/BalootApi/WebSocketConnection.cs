using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace BalootApi
{
    public class WebSocketConnection : MonoBehaviour
    {
        #region VAR
        [SerializeField] private List<SocketQueries> _socketQueries;
        private Dictionary<(string socketName, string evt), List<Action<string>>> _webglCallbacks = new();
#if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL: maintain a map of socket names to JS instance IDs
        private Dictionary<string, int> _webGLSockets = new Dictionary<string, int>();
#endif
        public event Action OnSocketConnected;

        // Connection state tracking
        private Dictionary<string, bool> _isConnected = new Dictionary<string, bool>();
        private bool _isApplicationFocused = true;
        #endregion

        #region ENGINE
        public void Connect()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // Disconnect existing WebGL sockets
            foreach (var kvp in _webGLSockets)
            {
                WebGL_Disconnect(kvp.Value);
            }

            foreach (var socketCfg in _socketQueries)
            {
                var queries = _socketQueries.ToDictionary(x => x.Parameter, x => x.Value);
                string queryString = string.Join("&", queries.Select(kv => kv.Key + "=" + Uri.EscapeDataString(kv.Value)));

                int instanceId = WebGL_CreateSocket(socketCfg.Value, queryString);
                _webGLSockets[socketCfg.Parameter] = instanceId;
                _isConnected[socketCfg.Parameter] = false;
            }
#endif
        }

        public void Connect(string socketName, List<SocketQueries> customQueries)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            var socketCfg = _socketQueries.FirstOrDefault(x => x.Parameter == socketName);
            if (socketCfg == null)
            {
                Debug.LogError($"Socket '{socketName}' not found in config");
                return;
            }

            var queries = customQueries.ToDictionary(x => x.Parameter, x => x.Value);
            string queryString = string.Join("&", queries.Select(kv => kv.Key + "=" + Uri.EscapeDataString(kv.Value)));

            if (_webGLSockets.TryGetValue(socketName, out var oldId))
                WebGL_Disconnect(oldId);

            int instanceId = WebGL_CreateSocket(socketCfg.Value, queryString);
            _webGLSockets[socketName] = instanceId;
            _isConnected[socketName] = false;
#endif
        }

        private void OnEnable()
        {
            Application.focusChanged += OnApplicationFocusChanged;
        }

        private void OnApplicationFocusChanged(bool hasFocus)
        {
            _isApplicationFocused = hasFocus;
            if (hasFocus)
            {
                Debug.Log("Application regained focus - checking socket connections...");
                CheckAndReconnectSockets();
            }
            else
            {
                Debug.Log("Application lost focus");
            }
        }

        private void OnDestroy()
        {
            Application.focusChanged -= OnApplicationFocusChanged;
#if UNITY_WEBGL && !UNITY_EDITOR
            foreach (var kvp in _webGLSockets)
                WebGL_Disconnect(kvp.Value);
#endif
        }
        #endregion

        #region MEMBER
        public void SubscribeTo(string eventName, string socketName, Action<string> callback)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (_webGLSockets.TryGetValue(socketName, out var id))
            {
                WebGL_On(id, eventName);
                var key = (socketName, eventName);
                if (!_webglCallbacks.ContainsKey(key))
                    _webglCallbacks[key] = new List<Action<string>>();
                _webglCallbacks[key].Add(callback);
            }
#endif
        }

        public void SendMessage(string eventName, string socketName, string json)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (_webGLSockets.TryGetValue(socketName, out var id))
            {
                if (!_isConnected[socketName])
                {
                    Debug.LogWarning($"WebGL socket '{socketName}' is not connected. Calling connect...");
                    WebGL_Connect(id);
                }
                WebGL_Emit(id, eventName, json);
            }
#endif
        }
        #endregion

        #region INTERNAL
        private void CheckAndReconnectSockets()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            foreach (var kvp in _webGLSockets)
            {
                var name = kvp.Key;
                if (!_isConnected.ContainsKey(name) || !_isConnected[name])
                {
                    Debug.Log($"Reconnecting WebGL socket: {name}");
                    WebGL_Connect(kvp.Value);
                }
            }
#endif
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        // JavaScript Interop
        [DllImport("__Internal")]
        private static extern int WebGL_CreateSocket(string url, string query);
        [DllImport("__Internal")]
        private static extern void WebGL_Connect(int instanceId);
        [DllImport("__Internal")]
        private static extern void WebGL_Disconnect(int instanceId);
        [DllImport("__Internal")]
        private static extern void WebGL_Emit(int instanceId, string eventName, string json);
        [DllImport("__Internal")]
        private static extern void WebGL_On(int instanceId, string eventName);

        // Called by JS via SendMessage
        private void OnWebGLEvent(string message)
        {
            var msg = JsonUtility.FromJson<WebGLMessage>(message);

            // Map instanceId back to your socketName
            var socketName = _webGLSockets.FirstOrDefault(kvp => kvp.Value == msg.instanceId).Key;
            if (socketName == null) return;

            switch (msg.eventName)
            {
                case "connect":
                    _isConnected[socketName] = true;
                    OnSocketConnected?.Invoke();
                    Debug.Log($"[WebGL] {socketName} connected");
                    break;
                case "disconnect":
                    _isConnected[socketName] = false;
                    Debug.Log($"[WebGL] {socketName} disconnected: {msg.data}");
                    break;
                case "error":
                    Debug.LogError($"[WebGL] {socketName} error: {msg.data}");
                    break;
                default:
                    // User‑level event: re‑emit via your subscription system
                    TriggerWebGLEventCallbacks(socketName, msg.eventName, msg.data);
                    break;
            }
        }

        private void TriggerWebGLEventCallbacks(string socket, string evt, string data)
        {
            var key = (socket, evt);
            if (_webglCallbacks.TryGetValue(key, out var list))
            {
                foreach (var cb in list) cb(data);
            }
        }

        [Serializable]
        private class WebGLMessage
        {
            public int instanceId;
            public string eventName;
            public string data;
        }
#endif
        #endregion
    }
}
