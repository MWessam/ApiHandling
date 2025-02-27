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
using VContainer;
using VContainer.Unity;
using Void = ApiHandling.Runtime.Void;

// using VContainer;

namespace BalootApi
{

    public interface IApiHandler
    {
        UniTask<Result<User>> GetUser(string userId, CancellationToken token = default);
        UniTask<Result<Void>> SendFriendRequest(User user, CancellationToken token = default);
        UniTask<Result<Void>> AcceptFriendRequest(User user, CancellationToken token = default);
        UniTask<Result<Void>> FollowUser(User user, CancellationToken token = default);
        UniTask<Result<Void>> UnfollowUser(User user, CancellationToken token = default);
        UniTask<Result<Void>> RemoveFriend(User user, CancellationToken token = default);
        UniTask<Result<Void>> BlockUser(User user, CancellationToken token = default);
        UniTask<Result<Void>> UnblockUser(User user, CancellationToken token = default);
        UniTask<Result<Void>> UpdateProfilePicture(Texture picture, CancellationToken token = default);
        UniTask<Result<Void>> UpdateStatus(string status, CancellationToken token = default);
        
        UniTask<Result<Void>> UpdateSelectedItem(CancellationToken token = default);
        UniTask<Result<Void>> UpdateSelectedItems(CancellationToken token = default);
        UniTask<Result<List<Item>>> GetUserSelectedItems(CancellationToken token = default);
        
        
        
        UniTask<Result<Inventory>> GetInventory(CancellationToken token = default);
        UniTask<Result<Void>> PurchaseItem(Item item, CancellationToken token = default);
        UniTask<Result<List<Item>>> GetUserStore(CancellationToken token = default);
        
        UniTask<Result<List<BaseNotification>>> GetNotifications(int page = 0, int pageSize = 30, CancellationToken token = default);
        UniTask<Result<Void>> MarkNotificationAsRead(BaseNotification notification, CancellationToken token = default);
        UniTask<Result<Void>> LikeComment(Comment comment, CancellationToken token = default);

        UniTask<Result<IEnumerable<ChatMessageEntity>>> GetChatPage(User receiver, int startIndex = 0,
            int pageSize = 10, CancellationToken token = default);
        UniTask<Result<Void>> SendChatMessage(User receiver, string message, CancellationToken token = default);


        UniTask<Result<IEnumerable<Post>>> GetUserFeed(int startIndex = 0, int pageSize = 10, CancellationToken token = default);
        UniTask<Result<IEnumerable<Post>>> GetPostsByUser(User user, int startIndex = 0, int pageSize = 10, CancellationToken token = default);
        UniTask<Result<Void>> CreatePost(string content, EPostType postType, CancellationToken token = default);
        UniTask<Result<Void>> HidePost(Post post, CancellationToken token = default);
        UniTask<Result<Void>> LikePost(Post post, CancellationToken token = default);
        UniTask<Result<IEnumerable<Post>>> GetPostsLikedByUser(int pageStart = 0, int pageSize = 10, CancellationToken token = default);
        UniTask<Result<Void>> CommentOnPost(Post post, string commentContent, CancellationToken token = default);
        
        UniTask<Result<Tournament>> CreateTournament(TimeSpan lifetime, int maxWinnerCount, CancellationToken token = default);

        UniTask<Result<Void>> FilterUser(CancellationToken token = default);

        UniTask<Result<Room>> CreateRoom(string roomName, int maxMemberCount = 30, bool isPrivate = true,
            TimeSpan lifetime = new TimeSpan(),
            int logoIndex = 0, CancellationToken token = default);

        UniTask<Result<List<Room>>> GetUserRooms(CancellationToken token = default);

        UniTask<Result<List<Tournament>>> GetTournaments(int page = 0, int pageSize = 10,
            CancellationToken token = default);

        UniTask<Result<Void>> UpdateMemberRole(Room room, User user, CancellationToken token = default);
        UniTask<Result<Void>> DeleteChatMessage(ChatMessageEntity message, CancellationToken token = default);
    }

    public abstract class BaseBalootApiCommand<T> : BaseApiCommand<T>
    {
        protected IApiHandler ApiHandler;

        public BaseBalootApiCommand()
        {
            ApiHandler = BalootLifetimeScope.Container.Resolve<IApiHandler>();
        }
    }


    public class BalootApiHandler : IApiHandler
    {
        private User _signedInUser;
        [Inject] private ApiRequest _apiRequest;

        // Repeated endpoint constants
        private const string UserEndpoint = "users";
        private const string UsersEndpoint = "users";
        private const string FriendsSendFriendRequestEndpoint = "friends/send-friend-request";
        private const string FriendsAcceptFriendRequestEndpoint = "friends/accept-friend-request";
        private const string FriendsUnfriendEndpoint = "friends/unfriend";
        private const string FollowingsEndpoint = "followings";
        private const string FollowingsUnfollowEndpoint = "followings/unfollow";
        private const string PlayerSelectedItemsEndpoint = "player-selected-items/player";
        private const string InventoryEndpoint = "inventory";
        private const string ItemBuyForEndpoint = "item/buy-for";
        private const string ItemsEndpoint = "item";
        private const string NotificationsEndpoint = "notifications/user";
        private const string CommentsEndpoint = "comments";
        private const string MessageEndpoint = "message";
        private const string PostsEndpoint = "posts";
        private const string GoldenPostEndpoint = "posts/golden";
        private const string RoomEndpoint = "/room";
        private const string AddMemberToRoomEndpoint = "/room/add-member";
        private const string TournamentEndpoint = "/tournament";
        private string UpdateMemberRoleEndpoint(int roomId, int memberId) => $"/room/{roomId}/member/{memberId}/role";
        private string GetUserRoomsEndpoint() => $"/room/user/{_signedInUser.Id}"; 

        // A cache for users, keyed by userId.
        private readonly Dictionary<string, (User user, DateTime cachedAt)> _userCache = new();

        // Cache expiration time for users (e.g., 10 minutes)
        private readonly TimeSpan _userCacheExpiration = TimeSpan.FromMinutes(10);

        // Optionally, you can also cache posts for a user. This key might combine userId and pagination.
        private readonly Dictionary<string, (IEnumerable<Post> posts, DateTime cachedAt)> _postsCache = new();

        // Cache expiration time for posts (e.g., 5 minutes)
        private readonly TimeSpan _postsCacheExpiration = TimeSpan.FromMinutes(5);

        public BalootApiHandler()
        {
            EventBus<OnUserLogin>.Register(OnUserLogin);
        }

        ~BalootApiHandler()
        {
            EventBus<OnUserLogin>.Deregister(OnUserLogin);
        }

        private void OnUserLogin(OnUserLogin obj)
        {
            _signedInUser = obj.User;
        }

        public async UniTask<Result<User>> GetUser(string userId, CancellationToken token = default)
        {
            // Check if the user is already in the cache and still valid
            if (_userCache.TryGetValue(userId, out var cacheEntry))
            {
                if (DateTime.UtcNow - cacheEntry.cachedAt < _userCacheExpiration)
                {
                    return Result<User>.Success(cacheEntry.user);
                }
                else
                {
                    // Optionally remove expired entry
                    _userCache.Remove(userId);
                }
            }
            var response = await _apiRequest.GetRequest($"{UserEndpoint}/{userId}", token);
            if (response.IsSuccess)
            {
                var userDto = JsonConvert.DeserializeObject<UserDto>(response.Value);
                var user = userDto.Adapt<User>();
                _userCache[userId] = (user, DateTime.UtcNow);
                return Result<User>.Success(user);
            }
            return Result<User>.Failure(response.ErrorMessage);
        }

        public async UniTask<Result<Void>> SendFriendRequest(User user, CancellationToken token = default)
        {
            var json = JsonConvert.SerializeObject(new SendFriendRequestDto(_signedInUser.Id, user.Id));
            return (await _apiRequest.PostRequest(FriendsSendFriendRequestEndpoint, json, token)).ToResult();
        }

        public async UniTask<Result<Void>> AcceptFriendRequest(User user, CancellationToken token = default)
        {
            var json = JsonConvert.SerializeObject(new SendFriendRequestDto(_signedInUser.Id, user.Id));
            return (await _apiRequest.PostRequest(FriendsAcceptFriendRequestEndpoint, json, token)).ToResult();
        }

        public async UniTask<Result<Void>> FollowUser(User user, CancellationToken token = default)
        {
            var json = JsonConvert.SerializeObject(new SendFriendRequestDto(_signedInUser.Id, user.Id));
            return (await _apiRequest.PostRequest(FollowingsEndpoint, json, token)).ToResult();
        }

        public async UniTask<Result<Void>> UnfollowUser(User user, CancellationToken token = default)
        {
            return (await _apiRequest.PostRequest($"{FollowingsUnfollowEndpoint}/{_signedInUser.Id}/{user.Id}", cancellationToken: token)).ToResult();
        }

        public async UniTask<Result<Void>> RemoveFriend(User user, CancellationToken token = default)
        {
            var result = await _apiRequest.DeleteRequest($"{FriendsUnfriendEndpoint}/{_signedInUser.Id}/{user.Id}");
            return result;
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
            var result = await _apiRequest.PatchRequestForm($"{UsersEndpoint}/{_signedInUser.Id}", token, formItems: pictureFormItem);
            return result.ToResult();
        }

        public async UniTask<Result<Void>> UpdateStatus(string status, CancellationToken token = default)
        {
            var statusFormItem = new FormItem("status", status, EFormItemType.StringValue);
            var result = await _apiRequest.PatchRequestForm($"{UsersEndpoint}/{_signedInUser.Id}", token, statusFormItem);
            return result.ToResult();
        }

        public async UniTask<Result<Void>> UpdateSelectedItem(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Void>> UpdateSelectedItems(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<List<Item>>> GetUserSelectedItems(CancellationToken token = default)
        {
            var response = await _apiRequest.GetRequest($"{PlayerSelectedItemsEndpoint}/{_signedInUser.Id}", token);
            if (response.IsSuccess)
            {
                var itemDtos = JsonConvert.DeserializeObject<List<ItemDto>>(response.Value);
                var items = itemDtos.Adapt<List<Item>>();
                return Result<List<Item>>.Success(items);
            }
            return Result<List<Item>>.Failure(response.ErrorMessage);
        }

        public async UniTask<Result<Inventory>> GetInventory(CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest($"{InventoryEndpoint}/{_signedInUser.Id}", token);
            if (result.IsSuccess)
            {
                List<ItemDto> itemDtos = JsonConvert.DeserializeObject<List<ItemDto>>(result.Value);
                var itemEntities = itemDtos.Adapt<List<Item>>();
                var inventory = new Inventory(itemEntities);
                _signedInUser.Inventory = inventory;
                return Result<Inventory>.Success(inventory);
            }
            return Result<Inventory>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<Void>> PurchaseItem(Item item, CancellationToken token = default)
        {
            var itemPurchaseDto = new ItemPurchaseDto(item.Id, 1);
            var result = await _apiRequest.PostRequest($"{ItemBuyForEndpoint}/{_signedInUser.Id}", JsonConvert.SerializeObject(itemPurchaseDto), token);
            return result.ToResult();
        }

        public async UniTask<Result<List<Item>>> GetUserStore(CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest($"{ItemsEndpoint}?page=0&page_size=30", token);
            if (!result.IsSuccess) return Result<List<Item>>.Failure(result.ErrorMessage);
            var itemDtos = JsonConvert.DeserializeObject<List<ItemDto>>(result.Value);
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

        public async UniTask<Result<List<BaseNotification>>> GetNotifications(int page = 0, int pageSize = 30, CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest($"{NotificationsEndpoint}/{_signedInUser.Id}?page={page}&page_size={pageSize}");
            if (result.IsSuccess)
            {
                var notificationsDto = JsonConvert.DeserializeObject<List<NotificationDto>>(result.Value);
                return Result<List<BaseNotification>>.Success(notificationsDto.Adapt<List<BaseNotification>>());
            }
            return Result<List<BaseNotification>>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<Void>> MarkNotificationAsRead(BaseNotification notification, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Void>> LikeComment(Comment comment, CancellationToken token = default)
        {
            var result = await _apiRequest.PostRequest($"{CommentsEndpoint}/{comment.Id}/like/{_signedInUser.Id}", cancellationToken: token);
            if (result.IsSuccess)
            {
                comment.LikeCount = JsonConvert.DeserializeObject<int>(result.Value);
                return Result<Comment>.Success(comment);
            }
            return Result<Comment>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<IEnumerable<ChatMessageEntity>>> GetChatPage(User receiver, int startIndex = 0, int pageSize = 10, CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest($"{MessageEndpoint}/{_signedInUser.Id}/{receiver.Id}?page={startIndex}&page_size={pageSize}", token);
            if (result.IsSuccess)
            {
                var chatDto = JsonConvert.DeserializeObject<List<ChatMessageDto>>(result.Value);
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

        public async UniTask<Result<Void>> SendChatMessage(User receiver, string message, CancellationToken token = default)
        {
            var messageDto = new ChatMessageDto()
            {
                Content = message,
                User1Id = _signedInUser.Id,
                User2Id = receiver.Id
            };
            var result = await _apiRequest.PostRequest(MessageEndpoint, JsonConvert.SerializeObject(messageDto), token);
            return result.ToVoidResult();
        }
        public async UniTask<Result<Void>> DeleteChatMessage(ChatMessageEntity message, CancellationToken token = default)
        {
            return await _apiRequest.DeleteRequest($"{MessageEndpoint}/{message.Id}", token);
        }

        public async UniTask<Result<IEnumerable<Post>>> GetUserFeed(int startIndex = 0, int pageSize = 10, CancellationToken token = default)
        {
            var response = await _apiRequest.GetRequest($"{PostsEndpoint}/user/{_signedInUser.Id}?page={startIndex}&page_size={pageSize}", token);
            if (response.IsSuccess)
            {
                var postDtos = JsonConvert.DeserializeObject<List<PostDto>>(response.Value);
                var postEntities = postDtos.Adapt<List<Post>>();
                for (var i = 0; i < postEntities.Count; i++)
                {
                    var dto = postDtos[i];
                    var entity = postEntities[i];
                    
                    
                }
                return Result<IEnumerable<Post>>.Success(postEntities);
            }
            return Result<IEnumerable<Post>>.Failure(response.ErrorMessage);
        }

        public async UniTask<Result<IEnumerable<Post>>> GetPostsByUser(User user, int startIndex = 0, int pageSize = 10, CancellationToken token = default)
        {
            var response = await _apiRequest.GetRequest($"{PostsEndpoint}/user/{user.Id}?page={startIndex}&page_size={pageSize}", token);
            if (response.IsSuccess)
            {
                var postDtos = JsonConvert.DeserializeObject<List<PostDto>>(response.Value);
                var postEntities = postDtos.Adapt<List<Post>>();
                for (var i = 0; i < postEntities.Count; i++)
                {
                    // Additional processing for each post if necessary
                }
                return Result<IEnumerable<Post>>.Success(postEntities);
            }
            return Result<IEnumerable<Post>>.Failure(response.ErrorMessage);
        }

        public async UniTask<Result<Void>> CreatePost(string content, EPostType postType, CancellationToken token = default)
        {
            var postCreationDto = new PostCreationDTO
            {
                Content = content,
                UserId = _signedInUser.Id,
            };
            if (postType == EPostType.Normal)
            {
                var postResult = await _apiRequest.PostRequest(PostsEndpoint, JsonConvert.SerializeObject(postCreationDto), token);
                return postResult;
            }
            else
            {
                var postResult = await _apiRequest.PostRequest(GoldenPostEndpoint, JsonConvert.SerializeObject(postCreationDto), token);
                return postResult;
            }
        }

        public async UniTask<Result<Void>> HidePost(Post post, CancellationToken token = default)
        {
            var result = await _apiRequest.PostRequest($"{PostsEndpoint}/{post.Id}/hide/{_signedInUser.Id}", "", token);
            return result;
        }

        public async UniTask<Result<Void>> LikePost(Post post, CancellationToken token = default)
        {
            var result = await _apiRequest.PostRequest($"{PostsEndpoint}/{post.Id}/like/{_signedInUser.Id}", "", token);
            return result;
        }

        public async UniTask<Result<IEnumerable<Post>>> GetPostsLikedByUser(int pageStart = 0, int pageSize = 10, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Void>> CommentOnPost(Post post, string commentContent, CancellationToken token = default)
        {
            var postCreationDto = new PostCreationDTO
            {
                Content = commentContent,
                UserId = _signedInUser.Id,
            };
            var result = await _apiRequest.PostRequest($"{PostsEndpoint}/{post.Id}/comment", JsonConvert.SerializeObject(postCreationDto), token);
            return result;
        }

        public async UniTask<Result<Tournament>> CreateTournament(TimeSpan lifetime, int maxWinnerCount, CancellationToken token = default)
        {
            var tournamentCreationDto = new TournamentCreationDto
            {
                Lifetime = (int)lifetime.TotalDays,
                MaxWinnersCount = maxWinnerCount
            };
            var result = await _apiRequest.PostRequest($"{TournamentEndpoint}",
                JsonConvert.SerializeObject(tournamentCreationDto), token);
            if (result.IsSuccess)
            {
                var tournamentDto = JsonConvert.DeserializeObject<TournamentDto>(result.Value);
                var tournament = tournamentDto.Adapt<Tournament>();
                return Result<Tournament>.Success(tournament);
            }
            else
            {
                return Result<Tournament>.Failure(result.ErrorMessage);
            }
        }

        public async UniTask<Result<List<Tournament>>> GetTournaments(int page = 0, int pageSize = 10,
            CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest($"{TournamentEndpoint}", token);
            if (result.IsSuccess)
            {
                var tournamentDtos = JsonConvert.DeserializeObject<List<TournamentDto>>(result.Value);
                var tournaments = tournamentDtos.Adapt<List<Tournament>>();
                for (int i = 0; i < tournamentDtos.Count; i++)
                {
                    var tournamentDto = tournamentDtos[i];
                    var tournament = tournaments[i];
                    if (tournamentDto.WinnerIds.Count == 0) continue;
                    var winners = new List<User>();
                    foreach (var winnerId in tournamentDto.WinnerIds)
                    {
                        await RequestWithRetry(() => GetUser(winnerId.ToString(), token), (v) => { winners.Add(v); },
                            token);
                    }

                    if (tournament.Winners.Count != tournamentDto.WinnerIds.Count)
                    {
                        return Result<List<Tournament>>.Failure(EResultError.PartialSuccess, "Couldn't parse winners of the tournament.");
                    }
                }
                return Result<List<Tournament>>.Success(tournaments);
            }
            else
            {
                return Result<List<Tournament>>.Failure(result.ErrorMessage);
            }
        }
        public async UniTask<Result<Void>> FilterUser(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Room>> CreateRoom(string roomName, int maxMemberCount = 30, bool isPrivate = true, TimeSpan lifetime = new TimeSpan(),
            int logoIndex = 0, CancellationToken token = default)
        {
            var roomDto = new RoomDto
            {
                Name = roomName,
                IsPrivate = isPrivate,
                Lifetime = (int)lifetime.TotalHours,
                MaxMemberCount = maxMemberCount,
                Logo = logoIndex,
                OwnerId = _signedInUser.Id
            };
            var result = await _apiRequest.PostRequest(RoomEndpoint, JsonConvert.SerializeObject(roomDto), token);
            if (result.IsSuccess)
            {
                var room = JsonConvert.DeserializeObject<RoomDto>(result.Value).Adapt<Room>();
                room.Owner = _signedInUser;
                return Result<Room>.Success(room);
            }
            return Result<Room>.Failure(result.ErrorMessage);
        }
        public async UniTask<Result<List<Room>>> GetUserRooms(CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest(GetUserRoomsEndpoint(), token);
            if (result.IsSuccess)
            {
                var roomsDto = JsonConvert.DeserializeObject<List<RoomDto>>(result.Value);
                if (roomsDto.Count == 0)
                {
                    return Result<List<Room>>.Success(new());
                }
                
                var rooms = roomsDto.Adapt<List<Room>>();
                var successfulRooms = new List<Room>();
                for (int i = 0; i < roomsDto.Count; ++i)
                {
                    var dto = roomsDto[i];
                    var room = rooms[i];
                    await RequestWithRetry(() => GetUser(dto.OwnerId.ToString(), token),
                        (v) => {
                            room.Owner = v;
                            successfulRooms.Add(room);
                        }, token);
                }

                if (successfulRooms.Count == 0)
                {
                    return Result<List<Room>>.Failure(EResultError.PartialSuccess,
                        "Failed to parse the returned rooms.");
                }
                return Result<List<Room>>.Success(successfulRooms);
            }
            return Result<List<Room>>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<Void>> UpdateMemberRole(Room room, User user, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        private async UniTask<Result<T>> RequestWithRetry<T>(Func<UniTask<Result<T>>> requestFunc, Action<T> onRequestSuccess, CancellationToken token = default)
        {
            var retryCount = 3;
            while (retryCount-- > 0)
            {
                        
                var ownerResult = await requestFunc();
                if (ownerResult.IsSuccess)
                {
                    onRequestSuccess(ownerResult.Value);
                    break;
                }
            }

            return Result<T>.Failure(EResultError.MaxRetryLimit, "Retried the request multiple times and failed.");
        }
    }


    public struct OnUserLogin : IEvent
    {
        public User User { get; }

        public OnUserLogin(User user)
        {
            User = user;
        }
    }
}