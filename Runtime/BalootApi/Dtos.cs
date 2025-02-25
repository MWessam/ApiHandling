using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BalootApi
{
    #region User

    [Serializable]
    public struct UserDTO
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("points")] public int Points { get; set; }
        [JsonProperty("won_matches_count")] public int WonMatchesCount { get; set; }
        [JsonProperty("lost_matches_count")] public int LostMatchesCount { get; set; }
        [JsonProperty("rank")] public int Rank { get; set; }
        [JsonProperty("balance")] public int Balance { get; set; }
        [JsonProperty("highest_reached_rank")] public int HighestRankReached { get; set; }
        [JsonProperty("status")] public string Status;
        [JsonProperty("photo_url")] public string PhotoUrl;
    }
    [Serializable]
    public struct ProfilePictureDTO
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
    public struct ItemDTO
    {
        public int Id;
        public int Price;
    }
    [Serializable]
    public struct InventoryDTO
    {
    }
    [Serializable]
    public struct ItemPurchaseDTO
    {
        public int Quantity { get; set; }

        public int ItemId { get; set; }
        public ItemPurchaseDTO(int itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }
    }


    #endregion

    #region Chat

    public struct ChatMessageDTO
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
    public struct NotificationsDTO
    {
    }
    [Serializable]
    public class NotificationDTO
    {
        [JsonProperty("id")]
        public int Id;
        [JsonProperty("sender_id")]
        public int SenderId;
        [JsonProperty("receiver_id")]
        public int ReceiverId;
        [JsonProperty("type")]
        public ENotificationType Type;
        [JsonProperty("content")]
        public string Content;

    }

    #endregion

    #region Comments

    public struct CommentDTO
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

    public struct PostDTO
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
        public List<CommentDTO> Comments { get; set; }
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
    
}