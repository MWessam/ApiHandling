using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ApiHandling.Runtime;
using ApiHandling.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using Mapster;
using Newtonsoft.Json;
using UnityEngine;
using Void = ApiHandling.Runtime.Void;

// using VContainer;

namespace BalootApi
{

    public interface IApiHandler
    {
        UniTask<Result<Void>> SendFriendRequest(User user, CancellationToken token = default);
        UniTask<Result<Void>> AcceptFriendRequest(User user, CancellationToken token = default);
        UniTask<Result<Void>> FollowUser(User user, CancellationToken token = default);
        UniTask<Result<Void>> UnfollowUser(User user, CancellationToken token = default);
        UniTask<Result<Void>> RemoveFriend(User user, CancellationToken token = default);
        UniTask<Result<Void>> BlockUser(User user, CancellationToken token = default);
        UniTask<Result<Void>> UnblockUser(User user, CancellationToken token = default);
        UniTask<Result<Void>> UpdateProfilePicture(Texture picture, CancellationToken token = default);
        UniTask<Result<Void>> UpdateStatus(string status, CancellationToken token = default);
        
        
        UniTask<Result<Inventory>> GetInventory(CancellationToken token = default);
        UniTask<Result<Void>> PurchaseItem(Item item, CancellationToken token = default);
        UniTask<Result<List<Item>>> GetUserStore(CancellationToken token = default);
        
        UniTask<Result<List<BaseNotification>>> GetNotifications(int page = 0, int pageSize = 30, CancellationToken token = default);
        UniTask<Result<Void>> MarkNotificationAsRead(BaseNotification notification, CancellationToken token = default);
        UniTask<Result<Comment>> LikeComment(Comment comment, CancellationToken token = default);

        UniTask<Result<IEnumerable<ChatMessageEntity>>> GetChatPage(User receiver, int startIndex = 0,
            int pageSize = 10, CancellationToken token = default);

        UniTask<Result<IEnumerable<Post>>> GetUserFeed(int startIndex = 0, int pageSize = 10, CancellationToken token = default);
        UniTask<Result<Post>> CreatePost(string content, EPostType postType, CancellationToken token = default);
        UniTask<Result<Void>> HidePost(Post post, CancellationToken token = default);
        UniTask<Result<Post>> LikePost(Post post, CancellationToken token = default);
        UniTask<Result<IEnumerable<Post>>> GetPostsLikedByUser(int pageStart = 0, int pageSize = 10, CancellationToken token = default);
        UniTask<Result<Comment>> CommentOnPost(Post post, string commentContent, CancellationToken token = default);
    }

    public abstract class BaseBalootApiCommand<T> : BaseApiCommand<T>
    {
        protected IApiHandler ApiHandler;
    }


    public class GamsaroApiHandler : IApiHandler
    { 
        private User _signedInUser;
        private ApiRequest _apiRequest;
        [GenerateApiCommand]
        public async UniTask<Result<Void>> SendFriendRequest(User user, CancellationToken token = default)
        {
            var json = JsonConvert.SerializeObject(new SendFriendRequestDto(_signedInUser.Id, user.Id));
            return (await _apiRequest.PostRequest("friends/send-friend-request", json, token)).ToResult();
        }

        public async UniTask<Result<Void>> AcceptFriendRequest(User user, CancellationToken token = default)
        {
            var json = JsonConvert.SerializeObject(new SendFriendRequestDto(_signedInUser.Id, user.Id));
            return (await _apiRequest.PostRequest("friends/accept-friend-request", json, token)).ToResult();
        }

        public async UniTask<Result<Void>> FollowUser(User user, CancellationToken token = default)
        {
            var json = JsonConvert.SerializeObject(new SendFriendRequestDto(_signedInUser.Id, user.Id));
            return (await _apiRequest.PostRequest("followings", json, token)).ToResult();
        }

        public async UniTask<Result<Void>> UnfollowUser(User user, CancellationToken token = default)
        {
            return (await _apiRequest.PostRequest($"followings/unfollow{_signedInUser.Id}/{user.Id}", cancellationToken:token)).ToResult();
        }

        public async UniTask<Result<Void>> RemoveFriend(User user, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Void>> BlockUser(User user, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Void>> UnblockUser(User user, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Void>> UpdateProfilePicture(Texture picture, CancellationToken token = default)
        {
            var pictureFormItem = new FormItem("image", SerializationUtilities.SerializeToByteArr(picture), EFormItemType.ByteArray);
            var result = await _apiRequest.PatchRequestForm($"users/{_signedInUser.Id}", token, formItems: pictureFormItem);
            return result.ToResult();
        }

        public async UniTask<Result<Void>> UpdateStatus(string status, CancellationToken token = default)
        {
            var statusFormItem = new FormItem("status", status, EFormItemType.StringValue);
            var result = await _apiRequest.PatchRequestForm($"users/{_signedInUser.Id}", token, statusFormItem);
            return result.ToResult();
        }

        public async UniTask<Result<Inventory>> GetInventory(CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest($"inventory/{_signedInUser.Id}", token);
            if (result.IsSuccess)
            {
                List<ItemDTO> itemDtos = JsonConvert.DeserializeObject<List<ItemDTO>>(result.Value);
                var itemEntities = itemDtos.Adapt<List<Item>>();
                var inventory = new Inventory(itemEntities);
                _signedInUser.Inventory = inventory;
                return Result<Inventory>.Success(inventory);
            }
            return Result<Inventory>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<Void>> PurchaseItem(Item item, CancellationToken token = default)
        {
            var itemPurchaseDto = new ItemPurchaseDTO(item.Id, 1);
            var result = await _apiRequest.PostRequest($"item/buy-for/{_signedInUser.Id}", JsonConvert.SerializeObject(itemPurchaseDto), token);
            return result.ToResult();
        }

        public async UniTask<Result<List<Item>>> GetUserStore(CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest($"item?page=0&page_size=30", token);
            if (!result.IsSuccess) return Result<List<Item>>.Failure(result.ErrorMessage);
            var itemDtos = JsonConvert.DeserializeObject<List<ItemDTO>>(result.Value);
            var itemEntities = itemDtos.Adapt<List<Item>>();
            if (_signedInUser.Inventory == null)
            {
                var inventoryResult = await GetInventory(token);
                if (inventoryResult.IsSuccess)
                {
                    var storeItems = itemEntities;
                    return Result<List<Item>>.Success(storeItems);
                }
                else
                {
                    return Result<List<Item>>.Failure(inventoryResult.ErrorMessage);
                }
            }
            else
            {
                var storeItems = itemEntities;
                return Result<List<Item>>.Success(storeItems);
            }
        }

        public async UniTask<Result<List<BaseNotification>>> GetNotifications(int page = 0, int pageSize = 30,
            CancellationToken token = default)
        {
            var result = await _apiRequest
                .GetRequest($"notifications/user/{_signedInUser.Id}?page={page}&page_size={pageSize}");
            if (result.IsSuccess)
            {
                var notificationsDto = JsonConvert.DeserializeObject<List<NotificationDTO>>(result.Value);
                return Result<List<BaseNotification>>.Success(notificationsDto.Adapt<List<BaseNotification>>());
            }
            return Result<List<BaseNotification>>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<Void>> MarkNotificationAsRead(BaseNotification notification, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Comment>> LikeComment(Comment comment, CancellationToken token = default)
        {
            var result = await _apiRequest.PostRequest($"comments/{comment.Id}/like/{_signedInUser.Id}", cancellationToken:token);
            if (result.IsSuccess)
            {
                comment.LikeCount = JsonConvert.DeserializeObject<int>(result.Value);
                return Result<Comment>.Success(comment);
            }
            return Result<Comment>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<IEnumerable<ChatMessageEntity>>> GetChatPage(User receiver, int startIndex = 0, int pageSize = 10, CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest($"message/{_signedInUser.Id}/{receiver.Id}?page={startIndex}&page_size={pageSize}");
            if (result.IsSuccess)
            {
                var chatDto = JsonConvert.DeserializeObject<List<ChatMessageDTO>>(result.Value);
                var chat = chatDto.Adapt<List<ChatMessageEntity>>();
                for (int i = 0; i < chat.Count; ++i)
                {
                    var dto = chatDto[i];
                    var entity = chat[i]; 
                    entity.User1 = dto.User1Id == _signedInUser.Id ? _signedInUser : receiver;
                    entity.User2 = dto.User2Id == _signedInUser.Id ? receiver : _signedInUser;
                }
                return Result<IEnumerable<ChatMessageEntity>>.Success(chat);
            }
            return Result<IEnumerable<ChatMessageEntity>>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<IEnumerable<Post>>> GetUserFeed(int startIndex = 0, int pageSize = 10, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Post>> CreatePost(string content, EPostType postType, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Void>> HidePost(Post post, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Post>> LikePost(Post post, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<IEnumerable<Post>>> GetPostsLikedByUser(int pageStart = 0, int pageSize = 10, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Comment>> CommentOnPost(Post post, string commentContent, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }




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
        public int Id { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public int WonMatchesCount { get; set; }
        public int LostMatchesCount { get; set; }
        public int HighestRankReached { get; set; }
        public int Rank { get; set; }
        public Texture ProfilePic { get; set; }
        public string Status { get; set; }
        public int Balance { get; set; }
        public List<User> Friends;
        public bool IsFollowing;
        public bool IsFriend;

        public List<BaseNotification> Notifications;

        public Inventory Inventory { get; set; }
        public int FollowerCount;
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
}