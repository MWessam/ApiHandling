using System.Collections.Generic;
using ApiHandling.Generated.Facade;
using ApiHandling.Runtime.Utilities;
using ApiHandling.Runtime;
using Cysharp.Threading.Tasks;
using Mapster;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VContainer;

namespace BalootApi
{
    public abstract class BaseSocketHandler : MonoBehaviour
    {
        [Inject] protected WebSocketConnection SocketConnection;
        [Inject] protected ApiFacade ApiFacade; 
        private void OnEnable()
        {
            SocketConnection.OnSocketConnected += OnSocketConnected;
            ApiEventBus<OnUserLogin>.Register(OnLogin);
        }
        private void OnDisable()
        {
            SocketConnection.OnSocketConnected -= OnSocketConnected;
            ApiEventBus<OnUserLogin>.Deregister(OnLogin);

        }

        protected abstract void OnSocketConnected();
        protected abstract void OnLogin(OnUserLogin obj);
    }

    public class LobbySocketHandler : BaseSocketHandler
    {

        public LobbyEntity UpdatedLobbyEntity;
        #region ENGINE


        protected override void OnLogin(OnUserLogin obj)
        {
            SocketConnection.Connect("lobby", new()
            {
                new SocketQueries()
                {
                    Parameter = "playerId",
                    Value = obj.User.Id.ToString()
                }
            });
        }

        #endregion

        #region Member
        public void InvitePlayer(string userID)
        {
            SocketConnection.SendMessage(LobbyWebSocketEventNames.SEND_INVITE_PLAYER, "lobby", $@"{{ ""playerId"": {long.Parse(userID)}}}");

        }
        public void AcceptLobbyInvitation(string lobbyId)
        {
            SocketConnection.SendMessage(LobbyWebSocketEventNames.SEND_ACCEPT_LOBBY_INVITATION, "lobby", $@"{{ ""lobbyId"": {long.Parse(lobbyId)}}}");
        }

        public void KickUser(string userID)
        {
            SocketConnection.SendMessage(LobbyWebSocketEventNames.SEND_KICK_PLAYER, "lobby", $@"{{ ""playerId"": {long.Parse(userID)}}}");

        }

        public void LeaveLobby()
        {
            SocketConnection.SendMessage(LobbyWebSocketEventNames.SEND_LEAVE_LOBBY, "lobby", $@"{{ ""lobbyId"": 1}}");
        }

        public void AssignNewOwner(string userID)
        {
            SocketConnection.SendMessage(LobbyWebSocketEventNames.SEND_ASSIGN_NEW_OWNER, "lobby", $@"{{ ""playerId"": {long.Parse(userID)}}}");

        }

        public void StartMatchmaking(string ticketId)
        {
            SocketConnection.SendMessage(LobbyWebSocketEventNames.SEND_START_MATCHMAKING, "lobby", $@"{{ ""ticketId"": ""{ticketId}""}}");
        }

        public void SetReadyState(bool newReadyState,string lobbyId)
        {
            string boolean=newReadyState?"true":"false";
            SocketConnection.SendMessage(LobbyWebSocketEventNames.SEND_SET_READY_STATE, "lobby", $@"{{""ready"":{boolean},""lobbyId"":{long.Parse(lobbyId)}}}");
        }
        #endregion

        #region INTERNAL

        protected override void OnSocketConnected()
        {
            SocketConnection.SubscribeTo(LobbyWebSocketEventNames.LISTEN_LOBBY_INVITATION, "lobby", OnLobbyInvitationReceived);
            SocketConnection.SubscribeTo(LobbyWebSocketEventNames.LISTEN_NOTIFICATION, "lobby", OnNotificationReceived);
            SocketConnection.SubscribeTo(LobbyWebSocketEventNames.LISTEN_LEFT_LOBBY, "lobby", OnLeftLobby);
            SocketConnection.SubscribeTo(LobbyWebSocketEventNames.LISTEN_LOBBY_UPDATED, "lobby", OnLobbyUpdated);
            SocketConnection.SubscribeTo(LobbyWebSocketEventNames.LISTEN_KICKED_FROM_LOBBY, "lobby", OnKickedFromLobby);
            SocketConnection.SubscribeTo(LobbyWebSocketEventNames.LISTEN_PROPAGATE_MATCHMAKING_TICKET, "lobby",OnReceiveTicketId);
        }
        void OnReceiveTicketId(string response)
        {
            var str=response.ToString();
            Debug.Log(str);
            TicketResponseEntity ticketResponseEntity=JsonConvert.DeserializeObject<List<TicketResponseEntity>>(str)[0];
            ApiEventBus<OnReceiveTicketId>.Raise(new(ticketResponseEntity));
        }

        private void OnNotificationReceived(string obj)
        {
            OnNotificationReceivedAsync(obj).Forget();

        }
        private void OnLobbyInvitationReceived(string response)
        {
            OnLobbyInvitationReceivedAsync(response).Forget();
        }
        private async UniTask OnNotificationReceivedAsync(string response)
        {
            var str = response.ToString();
            var json = JArray.Parse(str);
            foreach (var jToken in json)
            {
                if (jToken is JObject jObject)
                {
                    var notificationDto = JsonConvert.DeserializeObject<NotificationDto>(jObject.ToString());
                    var notification = notificationDto.Adapt<BaseNotification>();
                    
                    ApiEventBus<NotificationReceivedEvent>.Raise(new(new(){notification}));
                    return;
                }
            }
            ApiEventBus<NotificationReceivedEvent>.Raise(new());
        }
        private async UniTask OnLobbyInvitationReceivedAsync(string obj)
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
            var result = await ApiFacade.GetUser(inviterPlayerId).Fetch();
            await UniTask.SwitchToMainThread();
            if (result.IsSuccess)
            {
                ApiEventBus<OnLobbyInvited>.Raise(new(result.Value, lobbyId));
            }
        }

        private void OnKickedFromLobby(string response)
        {
            ApiEventBus<OnLobbyLeft>.Raise(new());
        }

        private void OnLeftLobby(string response)
        {
            ApiEventBus<OnLobbyLeft>.Raise(new());
        }

        private void OnLobbyUpdated(string response)
        {
            OnLobbyUpdatedAsync(response).Forget();
        }
        private async UniTask OnLobbyUpdatedAsync(string response)
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
                var result = await ApiFacade.GetUser(lobbyItem.userId).Fetch();
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
            ApiEventBus<OnLobbyUpdated>.Raise(new(users, host));
        }
        

        #endregion
    }
}