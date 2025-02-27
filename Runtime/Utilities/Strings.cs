namespace ApiHandling.Runtime.Utilities
{
    public static class LobbyWebSocketEventNames
    {
        //SEND EVENTS
        public const string SEND_INVITE_PLAYER="invitePlayer";
        public const string SEND_ACCEPT_LOBBY_INVITATION="acceptLobbyInvitation";
        public const string SEND_KICK_PLAYER="kickPlayer";
        public const string SEND_LEAVE_LOBBY="leaveLobby";
        public const string SEND_ASSIGN_NEW_OWNER="assignNewOwner";
        public const string SEND_START_MATCHMAKING="startMatchMaking";
        public const string SEND_SET_READY_STATE="setReadyState";
        //LISTEN EVENTS
        public const string LISTEN_LOBBY_INVITATION="lobbyInvitation";
        public const string LISTEN_NOTIFICATION="notification";
        public const string LISTEN_KICKED_FROM_LOBBY="kickedFromLobby";
        public const string LISTEN_LEFT_LOBBY="leftLobby";
        public const string LISTEN_LOBBY_UPDATED="lobbyUpdated";
        public const string LISTEN_PROPAGATE_MATCHMAKING_TICKET="propagateMatchMakingTicket";
    }
}