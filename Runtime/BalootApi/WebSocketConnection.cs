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
        private SocketIOUnity _webSocket;
        public event Action OnSocketConnected;
        #endregion
        #region ENGINE

        public void Connect()
        {
            _webSocket?.Disconnect();
            _webSocket = new(_configSo.SocketUri, new SocketIOOptions()
            {
                Query = _socketQueries.ToDictionary(x => x.Parameter, x=> x.Value),
            });
            _webSocket.JsonSerializer = new NewtonsoftJsonSerializer();
            _webSocket.OnConnected += OnSocketOpen;
            _webSocket.OnError += OnSocketError;
            _webSocket.OnDisconnected += OnSocketClosed;
            _webSocket.OnReconnectAttempt += OnSocketReconnectAttempt;
            _webSocket.Connect();
        }

        public void Connect(List<SocketQueries> socketQueries)
        {
            _webSocket?.Disconnect();
            _webSocket = new(_configSo.SocketUri, new SocketIOOptions()
            {
                Query = socketQueries.ToDictionary(x => x.Parameter, x=> x.Value),
            });
            _webSocket.JsonSerializer = new NewtonsoftJsonSerializer();
            _webSocket.OnConnected += OnSocketOpen;
            _webSocket.OnError += OnSocketError;
            _webSocket.OnDisconnected += OnSocketClosed;
            _webSocket.OnReconnectAttempt += OnSocketReconnectAttempt;
            _webSocket.Connect();
            Debug.Log(_webSocket.ServerUri.AbsoluteUri);
        }
        private void OnEnable()
        {
        }

        private void Start()
        {

        }



        private void OnDestroy()
        {
            _webSocket?.Disconnect();
            _webSocket?.Dispose();
        }
        #endregion

        #region MEMBER

        public void SubscribeTo(string eventName, Action<SocketIOResponse> callback)
        {
            _webSocket.On(eventName, callback);
        }

        public async UniTask SendMessage(string eventName, string json)
        {
            try
            {
                await _webSocket.EmitStringAsJSONAsync(eventName, json);
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