using System.Collections.Generic;
using ApiHandling.Generated.Facade;
using ApiHandling.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using Mapster;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketIOClient;
using UnityEngine;
using VContainer;

namespace BalootApi
{
    public class LobbySocketHandler : MonoBehaviour
    {
        [Inject] private WebSocketConnection _socketConnection;
        public LobbyEntity UpdatedLobbyEntity;
        [SerializeField] string _lobbyWebSocketUrl;
        [SerializeField] int _testInvitedPlayerId;
        [SerializeField] int _testLobbyId;
        [Inject] private ApiFacade _apiFacade; 
        #region ENGINE

        private void OnEnable()
        {
            _socketConnection.OnSocketConnected += OnSocketConnected;
            EventBus<OnUserLogin>.Register(OnLogin);
        }
        private void OnDisable()
        {
            _socketConnection.OnSocketConnected -= OnSocketConnected;
            EventBus<OnUserLogin>.Deregister(OnLogin);
        }
        private void OnLogin(OnUserLogin obj)
        {
            _socketConnection.Connect(new()
            {
                new SocketQueries()
                {
                    Parameter = "playerId",
                    Value = obj.User.Id.ToString()
                }
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _socketConnection.SendMessage(LobbyWebSocketEventNames.SEND_INVITE_PLAYER, $@"{{ ""playerId"": {_testInvitedPlayerId} }}");
                Debug.Log("A");
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                SetReadyState(true,_testLobbyId.ToString());
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                StartMatchmaking("1234e3213r");
            }
        }

        #endregion

        #region Member
        public void InvitePlayer(string userID)
        {
            _socketConnection.SendMessage(LobbyWebSocketEventNames.SEND_INVITE_PLAYER, $@"{{ ""playerId"": {long.Parse(userID)}}}");

        }
        public void AcceptLobbyInvitation(string lobbyId)
        {
            _socketConnection.SendMessage(LobbyWebSocketEventNames.SEND_ACCEPT_LOBBY_INVITATION, $@"{{ ""lobbyId"": {long.Parse(lobbyId)}}}");
        }

        public void KickUser(string userID)
        {
            _socketConnection.SendMessage(LobbyWebSocketEventNames.SEND_KICK_PLAYER, $@"{{ ""playerId"": {long.Parse(userID)}}}");

        }

        public void LeaveLobby()
        {
            _socketConnection.SendMessage(LobbyWebSocketEventNames.SEND_LEAVE_LOBBY, $@"{{ ""lobbyId"": 1}}");
        }

        public void AssignNewOwner(string userID)
        {
            _socketConnection.SendMessage(LobbyWebSocketEventNames.SEND_ASSIGN_NEW_OWNER, $@"{{ ""playerId"": {long.Parse(userID)}}}");

        }

        public void StartMatchmaking(string ticketId)
        {
            _socketConnection.SendMessage(LobbyWebSocketEventNames.SEND_START_MATCHMAKING, $@"{{ ""ticketId"": ""{ticketId}""}}");
        }

        public void SetReadyState(bool newReadyState,string lobbyId)
        {
            string boolean=newReadyState?"true":"false";
            _socketConnection.SendMessage(LobbyWebSocketEventNames.SEND_SET_READY_STATE,$@"{{""ready"":{boolean},""lobbyId"":{long.Parse(lobbyId)}}}");
        }
        #endregion

        #region INTERNAL

        private void OnSocketConnected()
        {
            _socketConnection.SubscribeTo(LobbyWebSocketEventNames.LISTEN_LOBBY_INVITATION, OnLobbyInvitationReceived);
            _socketConnection.SubscribeTo(LobbyWebSocketEventNames.LISTEN_NOTIFICATION, OnNotificationReceived);
            _socketConnection.SubscribeTo(LobbyWebSocketEventNames.LISTEN_LEFT_LOBBY, OnLeftLobby);
            _socketConnection.SubscribeTo(LobbyWebSocketEventNames.LISTEN_LOBBY_UPDATED, OnLobbyUpdated);
            _socketConnection.SubscribeTo(LobbyWebSocketEventNames.LISTEN_KICKED_FROM_LOBBY, OnKickedFromLobby);
            _socketConnection.SubscribeTo(LobbyWebSocketEventNames.LISTEN_PROPAGATE_MATCHMAKING_TICKET,OnReceiveTicketId);
        }
        void OnReceiveTicketId(SocketIOResponse response)
        {
            var str=response.ToString();
            Debug.Log(str);
            TicketResponseEntity ticketResponseEntity=JsonConvert.DeserializeObject<List<TicketResponseEntity>>(str)[0];
            EventBus<OnReceiveTicketId>.Raise(new(ticketResponseEntity));
        }

        private void OnNotificationReceived(SocketIOResponse obj)
        {
            OnNotificationReceivedAsync(obj).Forget();

        }
        private void OnLobbyInvitationReceived(SocketIOResponse response)
        {
            OnLobbyInvitationReceivedAsync(response).Forget();
        }
        private async UniTask OnNotificationReceivedAsync(SocketIOResponse response)
        {
            var str = response.ToString();
            var json = JArray.Parse(str);
            foreach (var jToken in json)
            {
                if (jToken is JObject jObject)
                {
                    var notificationDto = JsonConvert.DeserializeObject<NotificationDto>(jObject.ToString());
                    var notification = notificationDto.Adapt<BaseNotification>();
                    
                    EventBus<NotificationReceivedEvent>.Raise(new(new(){notification}));
                    return;
                }
            }
            // EventBus<NotificationReceivedEvent>.Raise(new());
        }
        private async UniTask OnLobbyInvitationReceivedAsync(SocketIOResponse obj)
        {
            var str = obj.ToString();
            var json = JArray.Parse(str);
            string inviterPlayerId = default;
            string lobbyId = default;
            foreach (var jToken in json)
            {
                if (jToken is JObject jObject)
                {
                    if (jObject.TryGetValue("playerId", out var playerId))
                    {
                        inviterPlayerId = new((string)playerId);
                    }
                    if (jObject.TryGetValue("lobbyId", out var lobbyIdJobject))
                    {
                        lobbyId = (string)lobbyIdJobject;
                    }
                }
            }
            var result = await _apiFacade.GetUser(inviterPlayerId).Fetch();
            await UniTask.SwitchToMainThread();
            if (result.IsSuccess)
            {
                EventBus<OnLobbyInvited>.Raise(new(result.Value, lobbyId));
            }
        }

        private void OnKickedFromLobby(SocketIOResponse response)
        {
            EventBus<OnLobbyLeft>.Raise(new());
        }

        private void OnLeftLobby(SocketIOResponse response)
        {
            EventBus<OnLobbyLeft>.Raise(new());
        }

        private void OnLobbyUpdated(SocketIOResponse response)
        {
            OnLobbyUpdatedAsync(response).Forget();
        }
        private async UniTask OnLobbyUpdatedAsync(SocketIOResponse response)
        {
            var str = response.ToString();
            //Debug.Log($"OnLobbyUpdatedAsync:{str}");
            UpdatedLobbyEntity=JsonConvert.DeserializeObject<List<LobbyEntity>>(str)[0];

            var json = JArray.Parse(str);
            int inviterPlayerId = 0;
            string lobbyId = default;
            List<(string userId, bool isLeader)> lobbyStuff = new();
            foreach (var jToken in json)
            {
                if (jToken is JObject jObject)
                {
                    if (jObject.TryGetValue("lobby", out var lobbyData))
                    {
                        if (lobbyData is JArray lobbyArray)
                        {
                            foreach (var lobby in lobbyArray)
                            {
                                if (lobby is JObject lobbyJobject)
                                {
                                    var id = (string)lobbyJobject["id"];
                                    var isLeaderStr = (string)lobbyJobject["isLeader"];
                                    lobbyStuff.Add((id, isLeaderStr == "True"));
                                }
                            }
                            break;
                        }
                    }
                }
            }
            List<User> users = new();
            User host = null;
            foreach (var lobbyItem in lobbyStuff)
            {
                var result = await _apiFacade.GetUser(lobbyItem.userId).Fetch();
                if (result.IsSuccess)
                {
                    users.Add(result.Value);
                    if (lobbyItem.isLeader)
                    {
                        host = result.Value;
                    }
                }
            }
            await UniTask.SwitchToMainThread();
            EventBus<OnLobbyUpdated>.Raise(new(users, host));
        }
        

        #endregion
    }
}