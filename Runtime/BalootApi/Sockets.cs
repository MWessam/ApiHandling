using System;
using System.Collections.Generic;

namespace BalootApi
{
    [Serializable]
    public class SocketQueries
    {
        public string Parameter;
        public string Value;
    }

    public struct OnLobbyUpdated
    {
        public List<User> LobbyUsers;
        public User Host;

        public OnLobbyUpdated(List<User> lobbyUsers, User host)
        {
            LobbyUsers = lobbyUsers;
            Host = host;
        }
    }

    public struct OnLobbyLeft
    {
        private List<User> LobbyUsers;
    }

    public class OnLobbyKicked
    {
        private List<User> LobbyUsers;
    }

    public struct OnLobbyInvited
    {
        public User Inviter;
        public string LobbyId;

        public OnLobbyInvited(User inviterPlayer, string lobbyId)
        {
            Inviter = inviterPlayer;
            LobbyId = lobbyId;
        }
    }
    public struct OnReceiveTicketId
    {
        public TicketResponseEntity TicketResponseEntity;
        public OnReceiveTicketId(TicketResponseEntity ticketResponseEntity)
        {
            TicketResponseEntity=ticketResponseEntity;
        }
    }
    [Serializable]
    public class LobbyEntity
    {
        public List<LobbyMember> lobby;
    }
    [Serializable]
    public class TicketResponseEntity
    {
        public string ticketId;
    }
    public struct NotificationReceivedEvent
    {
        public List<BaseNotification> Notifications;

        public NotificationReceivedEvent(List<BaseNotification> notifications)
        {
            Notifications = notifications;
        }
    }
}