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
    [Flags]
    public enum EShowOptions
    {
        None = 0,
        ShowFlag = 1,
        ShowGender = 2,
        ShowAge = 4,
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
        public string CoverPhotoUrl { get; set; }
        public Texture2D ProfilePic { get; set; }
        public Texture2D CoverPhotoPic { get; set; }

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
        public ENationalityType Nationality { get; set; }

        // Additional properties from your original class:

        public List<User> Friends { get; set; } = new();
        public List<User> Followings { get; set; } = new();

        public bool IsFollowing { get; set; }

        public bool IsFriend { get; set; }

        public List<BaseNotification> Notifications { get; set; } = new();
        public List<Item> Inventory { get; set; } = new();
        public CharacterAvatarData AvatarData { get; set; }
        public DateTime Birthdate { get; set; }
        public bool ShowGender { get; set; }
        public bool ShowAge { get; set; }
        public bool ShowFlag { get; set; }
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
        public ENationalityType Nationality;
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
    [Serializable]
public enum ENationalityType
{
    Unknown = 0,
    Afghanistan = 1,
    Albania = 2,
    Algeria = 3,
    Andorra = 4,
    Angola = 5,
    AntiguaAndBarbuda = 6,
    Argentina = 7,
    Armenia = 8,
    Australia = 9,
    Austria = 10,
    Azerbaijan = 11,
    Bahamas = 12,
    Bahrain = 13,
    Bangladesh = 14,
    Barbados = 15,
    Belarus = 16,
    Belgium = 17,
    Belize = 18,
    Benin = 19,
    Bhutan = 20,
    Bolivia = 21,
    BosniaAndHerzegovina = 22,
    Botswana = 23,
    Brazil = 24,
    Brunei = 25,
    Bulgaria = 26,
    BurkinaFaso = 27,
    Burundi = 28,
    CaboVerde = 29,
    Cambodia = 30,
    Cameroon = 31,
    Canada = 32,
    CentralAfricanRepublic = 33,
    Chad = 34,
    Chile = 35,
    China = 36,
    Colombia = 37,
    Comoros = 38,
    CongoBrazzaville = 39,
    CongoKinshasa = 40,
    CostaRica = 41,
    Croatia = 42,
    Cuba = 43,
    Cyprus = 44,
    CzechRepublic = 45,
    Denmark = 46,
    Djibouti = 47,
    Dominica = 48,
    DominicanRepublic = 49,
    EastTimor = 50,
    Ecuador = 51,
    Egypt = 52,
    ElSalvador = 53,
    EquatorialGuinea = 54,
    Eritrea = 55,
    Estonia = 56,
    Eswatini = 57,
    Ethiopia = 58,
    Fiji = 59,
    Finland = 60,
    France = 61,
    Gabon = 62,
    Gambia = 63,
    Georgia = 64,
    Germany = 65,
    Ghana = 66,
    Greece = 67,
    Grenada = 68,
    Guatemala = 69,
    Guinea = 70,
    GuineaBissau = 71,
    Guyana = 72,
    Haiti = 73,
    Honduras = 74,
    Hungary = 75,
    Iceland = 76,
    India = 77,
    Indonesia = 78,
    Iran = 79,
    Iraq = 80,
    Ireland = 81,
    Italy = 82,
    IvoryCoast = 83,
    Jamaica = 84,
    Japan = 85,
    Jordan = 86,
    Kazakhstan = 87,
    Kenya = 88,
    Kiribati = 89,
    KoreaNorth = 90,
    KoreaSouth = 91,
    Kosovo = 92,
    Kuwait = 93,
    Kyrgyzstan = 94,
    Laos = 95,
    Latvia = 96,
    Lebanon = 97,
    Lesotho = 98,
    Liberia = 99,
    Libya = 100,
    Liechtenstein = 101,
    Lithuania = 102,
    Luxembourg = 103,
    Madagascar = 104,
    Malawi = 105,
    Malaysia = 106,
    Maldives = 107,
    Mali = 108,
    Malta = 109,
    MarshallIslands = 110,
    Mauritania = 111,
    Mauritius = 112,
    Mexico = 113,
    Micronesia = 114,
    Moldova = 115,
    Monaco = 116,
    Mongolia = 117,
    Montenegro = 118,
    Morocco = 119,
    Mozambique = 120,
    Myanmar = 121,
    Namibia = 122,
    Nauru = 123,
    Nepal = 124,
    Netherlands = 125,
    NewZealand = 126,
    Nicaragua = 127,
    Niger = 128,
    Nigeria = 129,
    NorthMacedonia = 130,
    Norway = 131,
    Oman = 132,
    Pakistan = 133,
    Palau = 134,
    Palestine = 135,
    Panama = 136,
    PapuaNewGuinea = 137,
    Paraguay = 138,
    Peru = 139,
    Philippines = 140,
    Poland = 141,
    Portugal = 142,
    Qatar = 143,
    Romania = 144,
    Russia = 145,
    Rwanda = 146,
    SaintKittsAndNevis = 147,
    SaintLucia = 148,
    SaintVincentAndGrenadines = 149,
    Samoa = 150,
    SanMarino = 151,
    SaoTomeAndPrincipe = 152,
    SaudiArabia = 153,
    Senegal = 154,
    Serbia = 155,
    Seychelles = 156,
    SierraLeone = 157,
    Singapore = 158,
    Slovakia = 159,
    Slovenia = 160,
    SolomonIslands = 161,
    Somalia = 162,
    SouthAfrica = 163,
    SouthSudan = 164,
    Spain = 165,
    SriLanka = 166,
    Sudan = 167,
    Suriname = 168,
    Sweden = 169,
    Switzerland = 170,
    Syria = 171,
    Taiwan = 172,
    Tajikistan = 173,
    Tanzania = 174,
    Thailand = 175,
    Togo = 176,
    Tonga = 177,
    TrinidadAndTobago = 178,
    Tunisia = 179,
    Turkey = 180,
    Turkmenistan = 181,
    Tuvalu = 182,
    Uganda = 183,
    Ukraine = 184,
    UnitedArabEmirates = 185,
    UnitedKingdom = 186,
    UnitedStates = 187,
    Uruguay = 188,
    Uzbekistan = 189,
    Vanuatu = 190,
    VaticanCity = 191,
    Venezuela = 192,
    Vietnam = 193,
    Yemen = 194,
    Zambia = 195,
    Zimbabwe = 196
}
}
