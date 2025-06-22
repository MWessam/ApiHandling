using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine.Serialization;

namespace BalootApi
{
    #region User

    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct LoginDto
    {
        public string UserHandle;
        public string Password;
    }

    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct LoginResultDto
    {
        public UserDto User;
        public string AccessToken;
        public LoginResultDto(UserDto userDto, string accessToken)
        {
            User = userDto;
            AccessToken = accessToken;
        }

    }
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct RegisterDto
    {
        public string UserHandle;
        public string Password;
        public string Name;
        public string Email;
        public RegisterDto(string userHandle, string password, string name, string email = "")
        {
            UserHandle = userHandle;
            Password = password;
            Name = name;
            Email = email;
        }
    }
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct UserDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

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
        [JsonProperty("is_friend")]
        public bool IsFriend;
        [JsonProperty("is_following")]
        public bool IsFollowing;
        public string Email { get; set; }
        //Matches "nationality"
        public ENationalityType Nationality { get; set; }
    }
    [Serializable]
    public struct ProfilePictureDto
    {
    }
    [Serializable]
    public struct SendFriendRequestDto
    {
        [JsonProperty("user1_id")]
        public string SenderId;
        [JsonProperty("user2_id")]
        public int ReceiverId;

        public SendFriendRequestDto(string senderId, int receiverId)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
        }
    }
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct RelationDto
    {
        public string RelationType;
        public bool Accepted;
        public DateTime CreatedAt;
        public UserDto User;
    }

    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct FindRelationDto
    {
        public string Type;
    }

    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct IdsDto
    {
        [JsonProperty("ids")]
        public int[] Ids;
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
        public string ItemId;
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
        public string UserId { get; set; }

        public string ItemId { get; set; }
        public ItemSelectDto(string userId, string itemId)
        {
            UserId = userId;
            ItemId = itemId;
        }
    }

    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct CustomizationItemDto
    {
        public string Type;
        public ItemDto Item;
        public int ColorIndex;
    }
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct UpdateCustomizationItemDto
    {
        public string Type;
        public int ItemId;
        public int ColorIndex;
    }

    #endregion

    #region Chat

    public struct ChatMessageDto
    {
        [JsonProperty("id")] public string Id;
        [JsonProperty("content")] public string Content;
        [JsonProperty("user1_id")] public string User1Id;
        [JsonProperty("user2_id")] public string User2Id;
        [JsonProperty("created_at")] public string TimeStamp;
    }

    #endregion
    #region Notifications

    [Serializable]
    public struct NotificationsDto
    {
    }
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    [Serializable]
    public struct NotificationDto
    {
        public string Id;
        public UserDto Sender;
        public string ReceiverId;
        public string Type;
        public string Content;
        public string PostId;

    }

    #endregion

    #region Comments

    public struct CommentDto
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("content")]
        public string Content;
        [JsonProperty("created_at")]
        public DateTime TimeStamp;
        [JsonProperty("user_id")]
        public string UserId;
    }

    #endregion

    #region Posts

    public struct PostDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
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

        [JsonProperty("comments_count")]
        public int CommentsCount;
        [JsonProperty("user_id")]
        public string UserId { get; set; }

    }
    public struct PostCreationDTO
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct UpdatePlayerGameStateDto
    {
        public int Points;
        public int Coins;
        public bool IsRanking;
        public bool IsWinner;
    }

    #endregion

    #region Rooms
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct RoomDto
    {
        public string Id;
        public string Name;
        public int MaxMemberCount;
        public bool IsPrivate;
        public int Lifetime;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Logo;
        public string OwnerId;
        public bool IsOwner;
        public bool IsMember;
        public int MemberCount;
        public ENationalityType Nationality;
    }
    #endregion

    #region Tournament

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct TournamentDto
    {
        public string Id;
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
