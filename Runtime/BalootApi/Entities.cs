using System;
using System.Collections.Generic;
using UnityEngine;

namespace BalootApi
{
    [Serializable]
    public class Item
    {
        public string Name;
        public string Description;
        public Texture Icon;

        public int Price;

        // public EItemType ItemType;
        public int Id;
        public bool IsDefault;
    }


    [Serializable]
    public class Inventory
    {
        public List<Item> Items { get; private set; }
        public Inventory(List<Item> items) => Items = items;
    }



    [Serializable]
    public class User
    {
        // Matches "id"
        public int Id { get; set; }
        
        // Matches "name"
        public string Name { get; set; }
        
        // Matches "points"
        public float Points { get; set; }
        
        // Matches "played_matches_count"
        public int PlayedMatchesCount { get; set; }
        
        // Matches "won_matches_count"
        public int WonMatchesCount { get; set; }
        
        // Matches "lost_matches_count"
        public int LostMatchesCount { get; set; }
        
        // Matches "ranked_matches_count"
        public int RankedMatchesCount { get; set; }
        
        // Matches "player_rank"
        public int PlayerRank { get; set; }
        
        // Matches "highest_reached_rank"
        public int HighestRankReached { get; set; }
        
        // Matches "rank"
        public int Rank { get; set; }
        
        // Matches "photo_url" (changed from Texture to string for URL)
        public string PhotoUrl { get; set; }
        
        // Matches "status"
        public string Status { get; set; }
        
        // Matches "balance"
        public float Balance { get; set; }
        
        // Matches "vip_status"
        public bool VipStatus { get; set; }
        
        // Matches "availability_for_dm"
        public bool AvailabilityForDm { get; set; }
        
        // Matches "unique_credential_hash"
        public string UniqueCredentialHash { get; set; }
        
        // Matches "is_game_manager"
        public bool IsGameManager { get; set; }
        
        // Matches "cover_photo_id"
        public int CoverPhotoId { get; set; }
        
        // Matches "is_anonymous"
        public bool IsAnonymous { get; set; }
        
        // Matches "user_handle"
        public string UserHandle { get; set; }
        
        // Matches "is_male"
        public bool IsMale { get; set; }
        
        // Matches "facebook_profile_link"
        public string FacebookProfileLink { get; set; }
        
        // Matches "x_profile_link"
        public string XProfileLink { get; set; }
        
        // Matches "snapchat_profile_link"
        public string SnapchatProfileLink { get; set; }
        
        // Matches "instagram_profile_link"
        public string InstagramProfileLink { get; set; }
        
        // Matches "threads_profile_link"
        public string ThreadsProfileLink { get; set; }
        
        // Matches "bluesky_profile_link"
        public string BlueskyProfileLink { get; set; }
        
        // Matches "reddit_profile_link"
        public string RedditProfileLink { get; set; }
        
        // Matches "discord_handle"
        public string DiscordHandle { get; set; }
        
        // Matches "linkedin_profile_link"
        public string LinkedinProfileLink { get; set; }
        
        // Matches "youtube_channel_link"
        public string YoutubeChannelLink { get; set; }
        
        // Matches "whatsapp_number"
        public string WhatsappNumber { get; set; }
        
        // Matches "created_at"
        public DateTime CreatedAt { get; set; }
        
        // Matches "updated_at"
        public DateTime UpdatedAt { get; set; }
        
        // Matches "followers_count"
        public int FollowersCount { get; set; }
        
        // Additional properties from your original class:
        
        public List<User> Friends { get; set; }
        
        public bool IsFollowing { get; set; }
        
        public bool IsFriend { get; set; }
        
        public List<BaseNotification> Notifications { get; set; }
        
        public Inventory Inventory { get; set; }
    }







    [Serializable]
    public struct ChatMessageEntity
    {
        public int Id;
        public string Content;
        public User User1;
        public User User2;
        public DateTime TimeStamp;
    }

    public class Room
    {
        public string Name { get; set; }
        public int MaxMemberCount { get; set; }
        public bool IsPrivate { get; set; }
        public TimeSpan Lifetime { get; set; }
        public Sprite Logo { get; set; }
        public User Owner;
    }
    [Serializable]
    public class LobbyMember
    {
        public string Id;
        public bool IsLeader;
        public string TicketId;
        public string LobbyId;
        public long JoinedAt;
        public bool IsReady;
    }

    public class Tournament
    {
        public int Id;
        public TimeSpan Lifetime;
        public int MaxWinnersCount;
        public List<User> Winners;
    }
}