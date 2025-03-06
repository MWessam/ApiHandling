using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BalootApi
{
    #region User

    [Serializable]
    public struct UserDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        // Consider using a decimal/double if needed since JSON returns "100.00"
        [JsonProperty("points")]
        public float Points { get; set; }
        
        [JsonProperty("played_matches_count")]
        public int PlayedMatchesCount { get; set; }
        
        [JsonProperty("won_matches_count")]
        public int WonMatchesCount { get; set; }
        
        [JsonProperty("lost_matches_count")]
        public int LostMatchesCount { get; set; }
        
        [JsonProperty("ranked_matches_count")]
        public int RankedMatchesCount { get; set; }
        
        // Mapping player_rank from JSON to this property
        [JsonProperty("player_rank")]
        public int PlayerRank { get; set; }
        
        [JsonProperty("highest_reached_rank")]
        public int HighestRankReached { get; set; }
        
        // Consider using a decimal/double if needed since JSON returns "1000.00"
        [JsonProperty("balance")]
        public float Balance { get; set; }
        
        [JsonProperty("vip_status")]
        public bool VipStatus { get; set; }
        
        [JsonProperty("availability_for_dm")]
        public bool AvailabilityForDm { get; set; }
        
        [JsonProperty("unique_credential_hash")]
        public string UniqueCredentialHash { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("is_game_manager")]
        public bool IsGameManager { get; set; }
        
        [JsonProperty("cover_photo_id")]
        public int CoverPhotoId { get; set; }
        
        [JsonProperty("is_anonymous")]
        public bool IsAnonymous { get; set; }
        
        [JsonProperty("user_handle")]
        public string UserHandle { get; set; }
        
        [JsonProperty("is_male")]
        public bool IsMale { get; set; }
        
        [JsonProperty("facebook_profile_link")]
        public string FacebookProfileLink { get; set; }
        
        [JsonProperty("x_profile_link")]
        public string XProfileLink { get; set; }
        
        [JsonProperty("snapchat_profile_link")]
        public string SnapchatProfileLink { get; set; }
        
        [JsonProperty("instagram_profile_link")]
        public string InstagramProfileLink { get; set; }
        
        [JsonProperty("threads_profile_link")]
        public string ThreadsProfileLink { get; set; }
        
        [JsonProperty("bluesky_profile_link")]
        public string BlueskyProfileLink { get; set; }
        
        [JsonProperty("reddit_profile_link")]
        public string RedditProfileLink { get; set; }
        
        [JsonProperty("discord_handle")]
        public string DiscordHandle { get; set; }
        
        [JsonProperty("linkedin_profile_link")]
        public string LinkedinProfileLink { get; set; }
        
        [JsonProperty("youtube_channel_link")]
        public string YoutubeChannelLink { get; set; }
        
        [JsonProperty("whatsapp_number")]
        public string WhatsappNumber { get; set; }
        
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
        
        [JsonProperty("followers_count")]
        public int FollowersCount { get; set; }
        
        // This property is in your existing DTO though not found in the JSON sample.
        [JsonProperty("photo_url")]
        public string PhotoUrl { get; set; }
    }
    [Serializable]
    public struct ProfilePictureDto
    {
    }
    [Serializable]
    public struct SendFriendRequestDto
    {
        [JsonProperty("user1_id")]
        public int SenderId;
        [JsonProperty("user2_id")]
        public int ReceiverId;

        public SendFriendRequestDto(int senderId, int receiverId)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
        }
    }

    #endregion

    #region Items

    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct ItemDto
    {
        public int Id;
        [JsonConverter(typeof(StringToIntConverter))]
        public int Price;
        public int Quantity;
    }
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct InventoryItemDto
    {
        public int ItemId;
        public int Price;
        public int Quantity;
        public ItemDto Item;
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    [Serializable]
    public struct SelectedItemsDto
    {
        public ItemDto Item;
    }

    [Serializable]
    public struct InventoryDto
    {
    }

    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct ArrayDto<T>
    {
        public List<T> Data;
    }
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct ItemPurchaseDto
    {
        public int Quantity { get; set; }

        public int ItemId { get; set; }
        public ItemPurchaseDto(int itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }
    }
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct ItemSelectDto
    {
        public int UserId { get; set; }

        public int ItemId { get; set; }
        public ItemSelectDto(int userId, int itemId)
        {
            UserId = userId;
            ItemId = itemId;
        }
    }

    #endregion

    #region Chat

    public struct ChatMessageDto
    {
        [JsonProperty("id")] public int Id;
        [JsonProperty("content")] public string Content;
        [JsonProperty("user1_id")] public int User1Id;
        [JsonProperty("user2_id")] public int User2Id;
        [JsonProperty("created_at")] public string TimeStamp;
    }

    #endregion
    #region Notifications

    [Serializable]
    public struct NotificationsDto
    {
    }
    [Serializable]
    public struct NotificationDto
    {
        [JsonProperty("id")]
        public int Id;
        [JsonProperty("sender_id")]
        public int SenderId;
        [JsonProperty("receiver_id")]
        public int ReceiverId;
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("content")]
        public string Content;

    }

    #endregion

    #region Comments

    public struct CommentDto
    {
        [JsonProperty("id")]
        public int Id;
        [JsonProperty("content")]
        public string Content;
        [JsonProperty("created_at")]
        public DateTime TimeStamp;
        [JsonProperty("user_id")]
        public int UserId;
    }

    #endregion

    #region Posts

    public struct PostDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("created_at")]
        public DateTime PostCreationTime { get; set; }
        [JsonProperty("likes")] 
        public int LikesCount { get; set; }
        [JsonProperty("isLikedByUser")]
        public bool IsLiked { get; set; }
        [JsonProperty("comments")]
        public List<CommentDto> Comments { get; set; }
        [JsonProperty("user_id")]
        public int UserId { get; set; }
    }
    public struct PostCreationDTO
    {
        [JsonProperty("user_id")]
        public int UserId { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
    }

    #endregion

    #region Rooms
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct RoomDto
    {
        public string Name;
        public int MaxMemberCount;
        public bool IsPrivate;
        public int Lifetime;
        public int Logo;
        public int OwnerId;
    }
    #endregion

    #region Tournament

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct TournamentDto
    {
        public int Id;
        public int Lifetime;
        public int MaxWinnersCount;
        public List<int> WinnerIds;
    }
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct TournamentCreationDto
    {
        public int Lifetime;
        public int MaxWinnersCount;
    }


    #endregion
}