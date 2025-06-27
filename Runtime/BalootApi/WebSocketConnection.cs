using System;
using System.Collections.Generic;
using System.Linq;
using ApiHandling.Runtime;
using Cysharp.Threading.Tasks;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using VContainer;

namespace BalootApi
{
    public class WebSocketConnection : MonoBehaviour
    {
        #region VAR
        [Inject] private ApiConfigSO _configSo;
        [SerializeField] private List<SocketQueries> _socketQueries;
        private Dictionary<string, SocketIOUnity> _webSocket=new();
        public event Action OnSocketConnected;
        #endregion
        #region ENGINE

        public void Connect()
        {
            foreach (var socketKvp in _webSocket)
            {
                socketKvp.Value?.Disconnect();
            }

            foreach (var socket in _configSo.SocketUris)
            {
                var websocket = _webSocket[socket.SocketName] = new(socket.SocketUri, new SocketIOOptions()
                {
                    Query = _socketQueries.ToDictionary(x => x.Parameter, x=> x.Value),
                });
                
                websocket.JsonSerializer = new NewtonsoftJsonSerializer();
                websocket.OnConnected += OnSocketOpen;
                websocket.OnError += OnSocketError;
                websocket.OnDisconnected += OnSocketClosed;
                websocket.OnReconnectAttempt += OnSocketReconnectAttempt;
                websocket.Connect();
            }


        }

        public void Connect(string socketName, List<SocketQueries> socketQueries)
        {
            var socket = _configSo.SocketUris.FirstOrDefault(x => x.SocketName == socketName);
            if (socket == null)
            {
                Debug.LogError($"Found no socket of name: {socketName}");
                return;
            }
            if (_webSocket.TryGetValue(socket.SocketName, out var webSocket))
            {
                webSocket?.Disconnect();
            }
            _webSocket[socketName] = new(socket.SocketUri, new SocketIOOptions()
            {
                Query = socketQueries.ToDictionary(x => x.Parameter, x => x.Value),
            });
            var websocket = _webSocket[socketName];
            websocket.JsonSerializer = new NewtonsoftJsonSerializer();
            websocket.OnConnected += OnSocketOpen;
            websocket.OnError += OnSocketError;
            websocket.OnDisconnected += OnSocketClosed;
            websocket.OnReconnectAttempt += OnSocketReconnectAttempt;
            websocket.Connect();
            //Debug.Log(websocket.ServerUri.AbsoluteUri);
        }
        private void OnEnable()
        {
        }

        private void Start()
        {
        }



        private void OnDestroy()
        {
            foreach (var socket in _webSocket)
            {
                socket.Value?.Disconnect();
                socket.Value?.Dispose();
            }
        }
        #endregion

        #region MEMBER

        public void SubscribeTo(string eventName, string socketName, Action<SocketIOResponse> callback)
        {
            if (_webSocket.TryGetValue(socketName, out var socket))
            {
                socket.On(eventName, callback);
            }
        }

        public async UniTask SendMessage(string eventName, string socketName, string json)
        {
            try
            {
                if (_webSocket.TryGetValue(socketName, out var socket))
                {
                    await socket.EmitStringAsJSONAsync(eventName, json);
                }
                Debug.Log(eventName);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        #endregion
        #region INTERNAL
        private void OnSocketReconnectAttempt(object sender, int e)
        {
            
        }

        private void OnSocketOpen(object sender, EventArgs eventArgs)
        {
            Debug.Log($"Ze bluetoos device (websocket: {_configSo.SocketUri}) has connecteduh  sucksesfullay!");
            OnSocketConnected?.Invoke();
        }

        private void OnSocketClosed(object sender, string s)
        {
            Debug.Log($"Disconnected. {s}");
        }

        private void OnSocketError(object sender, string s)
        {
            Debug.LogError($"Socket error: {s}");
        }
        #endregion
    }
}