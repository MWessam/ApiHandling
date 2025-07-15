using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using Cysharp.Threading.Tasks;
using SocketIOClient;
using ApiHandling.Runtime;
using VContainer;
using SocketIOClient.Newtonsoft.Json;

namespace BalootApi
{
    public class WebSocketConnection : MonoBehaviour
    {
        #region VAR
        [Inject] private ApiConfigSO _configSo;
        [SerializeField] private List<SocketQueries> _socketQueries;
    #if UNITY_WEBGL && !UNITY_EDITOR
        // WebGL: maintain a map of socket names to JS instance IDs
        private Dictionary<string, int> _webGLSockets = new Dictionary<string, int>();
    #else
        // Desktop/Mobile: use SocketIOUnity client
        private Dictionary<string, SocketIOUnity> _webSocket = new Dictionary<string, SocketIOUnity>();
    #endif

        public event Action OnSocketConnected;

        // Reconnection settings
        [Header("Reconnection Settings")]
        [SerializeField] private bool _enableAutoReconnect = true;
        [SerializeField] private int _maxReconnectAttempts = 10;
        [SerializeField] private float _reconnectDelay = 1f;
        [SerializeField] private float _maxReconnectDelay = 30f;

        // Connection state tracking
        private Dictionary<string, bool> _isConnected = new Dictionary<string, bool>();
        private Dictionary<string, int> _reconnectAttempts = new Dictionary<string, int>();
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
    #else
            foreach (var socket in _webSocket.Values)
                socket?.Disconnect();
    #endif

            foreach (var socketCfg in _configSo.SocketUris)
            {
                var queries = _socketQueries.ToDictionary(x => x.Parameter, x => x.Value);
                string queryString = string.Join("&", queries.Select(kv => kv.Key + "=" + Uri.EscapeDataString(kv.Value)));

    #if UNITY_WEBGL && !UNITY_EDITOR
                int instanceId = WebGL_CreateSocket(socketCfg.SocketUri, queryString);
                _webGLSockets[socketCfg.SocketName] = instanceId;
                _isConnected[socketCfg.SocketName] = false;
                _reconnectAttempts[socketCfg.SocketName] = 0;
    #else
                var client = new SocketIOUnity(socketCfg.SocketUri, CreateSocketIOOptions());
                _webSocket[socketCfg.SocketName] = client;

                client.JsonSerializer = new NewtonsoftJsonSerializer();
                client.OnConnected += OnSocketOpen;
                client.OnError += OnSocketError;
                client.OnDisconnected += OnSocketClosed;
                client.OnReconnectAttempt += OnSocketReconnectAttempt;
                client.Connect();

                _isConnected[socketCfg.SocketName] = false;
                _reconnectAttempts[socketCfg.SocketName] = 0;
    #endif
            }
        }

        public void Connect(string socketName, List<SocketQueries> customQueries)
        {
            var socketCfg = _configSo.SocketUris.FirstOrDefault(x => x.SocketName == socketName);
            if (socketCfg == null)
            {
                Debug.LogError($"Socket '{socketName}' not found in config");
                return;
            }

            var queries = customQueries.ToDictionary(x => x.Parameter, x => x.Value);
            string queryString = string.Join("&", queries.Select(kv => kv.Key + "=" + Uri.EscapeDataString(kv.Value)));

    #if UNITY_WEBGL && !UNITY_EDITOR
            if (_webGLSockets.TryGetValue(socketName, out var oldId))
                WebGL_Disconnect(oldId);

            int instanceId = WebGL_CreateSocket(socketCfg.SocketUri, queryString);
            _webGLSockets[socketName] = instanceId;
            _isConnected[socketName] = false;
            _reconnectAttempts[socketName] = 0;
    #else
            if (_webSocket.TryGetValue(socketName, out var oldSocket))
                oldSocket?.Disconnect();

            var client = new SocketIOUnity(socketCfg.SocketUri, CreateSocketIOOptions(customQueries));
            _webSocket[socketName] = client;

            client.JsonSerializer = new NewtonsoftJsonSerializer();
            client.OnConnected += OnSocketOpen;
            client.OnError += OnSocketError;
            client.OnDisconnected += OnSocketClosed;
            client.OnReconnectAttempt += OnSocketReconnectAttempt;
            client.Connect();

            _isConnected[socketName] = false;
            _reconnectAttempts[socketName] = 0;
    #endif
        }

    #if !(UNITY_WEBGL && !UNITY_EDITOR)
        private SocketIOOptions CreateSocketIOOptions(List<SocketQueries> customQueries = null)
        {
            var queries = (customQueries ?? _socketQueries).ToDictionary(x => x.Parameter, x => x.Value);
            return new SocketIOOptions()
            {
                Query = queries
            };
        }
    #endif

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
    #else
            foreach (var socket in _webSocket.Values)
            {
                socket?.Disconnect();
                socket?.Dispose();
            }
    #endif
        }
        #endregion

        #region MEMBER
        public void SubscribeTo(string eventName, string socketName, Action<SocketIOResponse> callback)
        {
    #if UNITY_WEBGL && !UNITY_EDITOR
            if (_webGLSockets.TryGetValue(socketName, out var id))
            {
                WebGL_On(id, eventName);
                // JS will call back via SendMessage to OnWebGLEvent
            }
    #else
            if (_webSocket.TryGetValue(socketName, out var socket))
                socket.On(eventName, callback);
    #endif
        }

        public async UniTask SendMessage(string eventName, string socketName, string json)
        {
            try
            {
    #if UNITY_WEBGL && !UNITY_EDITOR
                if (_webGLSockets.TryGetValue(socketName, out var id))
                {
                    if (!_isConnected[socketName])
                    {
                        Debug.LogWarning($"WebGL socket '{socketName}' is not connected. Calling connect...");
                        WebGL_Connect(id);
                        await UniTask.Delay(TimeSpan.FromSeconds(1));
                    }
                    WebGL_Emit(id, eventName, json);
                }
    #else
                if (_webSocket.TryGetValue(socketName, out var socket))
                {
                    if (!_isConnected[socketName])
                    {
                        Debug.LogWarning($"Socket '{socketName}' is not connected. Attempting to reconnect...");
                        socket.Connect();
                        await UniTask.Delay(1000);
                    }
                    await socket.EmitStringAsJSONAsync(eventName, json);
                }
    #endif
                Debug.Log(eventName);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error sending message to {socketName}: {e.Message}");
            }
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
    #else
            foreach (var kvp in _webSocket)
            {
                var name = kvp.Key;
                var socket = kvp.Value;
                if (!_isConnected.ContainsKey(name) || !_isConnected[name])
                {
                    Debug.Log($"Reconnecting socket: {name}");
                    socket.Connect();
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
            // Parse message JSON: { instanceId, eventName, data }
            // Set _isConnected and invoke callbacks or OnSocketOpen/Closed accordingly
            // TODO: implement parsing and dispatching
        }
    #else
        private void OnSocketOpen(object sender, EventArgs e)
        {
            var client = sender as SocketIOUnity;
            var name = _webSocket.FirstOrDefault(x => x.Value == client).Key;
            if (name != null)
            {
                _isConnected[name] = true;
                _reconnectAttempts[name] = 0;
                Debug.Log($"Connection to socket '{name}' successful!");
            }
            OnSocketConnected?.Invoke();
        }

        private void OnSocketClosed(object sender, string reason)
        {
            var client = sender as SocketIOUnity;
            var name = _webSocket.FirstOrDefault(x => x.Value == client).Key;
            if (name != null)
            {
                _isConnected[name] = false;
                Debug.Log($"Socket '{name}' disconnected. Reason: {reason}");
                if (_isApplicationFocused && _enableAutoReconnect)
                    client.Connect();
            }
        }

        private void OnSocketError(object sender, string error)
        {
            var client = sender as SocketIOUnity;
            var name = _webSocket.FirstOrDefault(x => x.Value == client).Key;
            Debug.LogError($"Socket error{(name != null ? $" for '{name}'" : "")}: {error}");
        }

        private void OnSocketReconnectAttempt(object sender, int attempt)
        {
            var client = sender as SocketIOUnity;
            var name = _webSocket.FirstOrDefault(x => x.Value == client).Key;
            if (name != null)
            {
                _reconnectAttempts[name] = attempt;
                Debug.Log($"Reconnection attempt {attempt} for socket '{name}'");
            }
        }
    #endif
        #endregion
    }
}
