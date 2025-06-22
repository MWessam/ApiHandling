using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace BalootApi
{
    [Serializable]
    public class Item
    {
        public string Id;
        public string Name;
        public string Description;
        public Sprite Icon;
        public int Price;
        public EItemType ItemType;
        public bool IsDefault;
        public int Quantity;
        public bool IsConsumable;
        public bool IsEquippable;
    }
    public enum EItemType
    {
        None,
        Currency,
        Consumable,
        QuestItem,
        Weapon,
        Accessory,
        Helmet,
        Chest,
        Legs,
        Boots,
        Amulet,
        Maps,
        Cards,
        Subscription,
        Gold,
        Cosmetics


    }
    [Serializable]
    public class Inventory
    {
        public List<InventoryItem> Items { get; private set; }
        public Inventory(List<InventoryItem> items) => Items = items;
    }
    public class InventoryItem
    {
        public Item Item { get; set; }
        public Sprite Icon => Item.Icon;
        public bool IsEquippable { get; set; }
        public bool IsConsumable { get; set; }
    }
    [Serializable]
    public class User
    {
        // Matches "id"
        public string Id { get; set; }

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
        public Texture2D ProfilePic { get; set; }

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
        // Matches "email"
        public string Email { get; set; }
        //Matches "nationality"
        public int Nationality { get; set; }

        // Additional properties from your original class:

        public List<User> Friends { get; set; } = new();
        public List<User> Followings { get; set; } = new();

        public bool IsFollowing { get; set; }

        public bool IsFriend { get; set; }

        public List<BaseNotification> Notifications { get; set; } = new();

        public List<Item> Inventory { get; set; } = new();
        public CharacterAvatarData AvatarData { get; set; }
    }
    public class CreateUserEntity
    {
        public string LoginToken { get; set; }
        public string Name { get; set; } = "default";
        public string Status { get; set; } = "...";
        public bool VipStatus { get; set; }
        public bool AvailabilityForDm { get; set; }
    }
    [Serializable]
    public struct ChatMessage
    {
        public string Id;
        public string Content;
        [FormerlySerializedAs("User1")] public User Sender;
        [FormerlySerializedAs("User2")] public User Receiver;
        public DateTime TimeStamp;
    }

    public class Room
    {
        public string Id;
        public string Name { get; set; }
        public int MaxMemberCount { get; set; }
        public bool IsPrivate { get; set; }
        // public TimeSpan Lifetime { get; set; }
        public int Logo { get; set; }
        public bool IsOwner;
        public bool IsMember;
        public User Owner;
        public int MemberCount;
        public int Nationality;
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

    public enum ECustomization
    {
        Gender,
        Hair,
        EyeShape,
        EyeBrow,
        EyeShadow,
        Lips,
        Nose,
        Accessories,
        FacialHair,
        FaceShape,
        Headwear,
        Top,
        Shoes,
        Pants,
        BodyType,
        Ears
    }

    public class CharacterAvatarData : ICloneable
    {
        public Dictionary<ECustomization, ColorCustomizationItem> CustomizationItemsDictionary;

        public CharacterAvatarData(List<ColorCustomizationItem> customizationItems)
        {
            CustomizationItemsDictionary = customizationItems.ToDictionary(x => x.CustomizationType, x => (ColorCustomizationItem)x.Clone());
        }
        public CharacterAvatarData(Dictionary<ECustomization, ColorCustomizationItem> customizationItems)
        {
            CustomizationItemsDictionary = customizationItems;
        }
        public object Clone()
        {
            var characterAvatarData =
                new CharacterAvatarData(CustomizationItemsDictionary.ToDictionary(x => x.Key, x => x.Value));
            return characterAvatarData;
        }
    }
    [Serializable]
    public class ColorCustomizationItem : ICloneable
    {
        public string Index;
        public ECustomization CustomizationType;
        public int ColorIndex;
        public User User;
        public object Clone()
        {
            return new ColorCustomizationItem()
            {
                Index = Index,
                ColorIndex = ColorIndex,
                CustomizationType = CustomizationType,
                User = User
            };
        }
    }

}
