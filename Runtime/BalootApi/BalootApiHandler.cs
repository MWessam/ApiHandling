using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiHandling.Runtime;
using ApiHandling.Runtime.Utilities;
using Cysharp.Threading.Tasks;
using Mapster;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Void = ApiHandling.Runtime.Void;
using ApiHandling.Runtime.Utilities;
using UnityEngine.Networking;

namespace BalootApi
{

    public interface IApiHandler
    {
        UniTask<Result<User>> SignInWithToken(string loginToken, CancellationToken token = default);
        UniTask<Result<User>> SignInWithPassword(LoginDto loginDto, CancellationToken token = default);
        UniTask<Result<User>> Register(RegisterDto registerDto, CancellationToken token = default);
        UniTask<Result<User>> RegisterWithToken(string loginToken, RegisterDto registerDto, CancellationToken token = default);
        UniTask<Result<User>> GetUser(string userId, bool invalidateCache = false, CancellationToken token = default);
        UniTask<Result<List<User>>> GetUsers(int pageStart = 0, int pageSize = 10, string name = "", CancellationToken token = default);

        UniTask<Result<List<User>>> GetUsersByIds(IEnumerable<string> ids, int pageStart = 0, int pageSize = 10, CancellationToken token = default);
        UniTask<Result<List<User>>> GetFriendList(CancellationToken token = default);
        UniTask<Result<List<User>>> GetFriendRequests(int page = 0, int pageSize = 30,
            CancellationToken token = default);
        UniTask<Result<Void>> SendFriendRequest(User user, CancellationToken token = default);
        UniTask<Result<Void>> AcceptFriendRequest(User user, CancellationToken token = default);
        UniTask<Result<Void>> FollowUser(User user, CancellationToken token = default);
        UniTask<Result<Void>> UnfollowUser(User user, CancellationToken token = default);
        UniTask<Result<Void>> RemoveFriend(User user, CancellationToken token = default);
        UniTask<Result<Void>> BlockUser(User user, CancellationToken token = default);
        UniTask<Result<Void>> UnblockUser(User user, CancellationToken token = default);
        UniTask<Result<Void>> UpdateProfilePicture(Texture picture, CancellationToken token = default);
        UniTask<Result<Void>> UpdateStatus(string status, CancellationToken token = default);

        UniTask<Result<Void>> UpdatePlayerGameState(string userId, int coins, int points, bool isWinner, bool isRanking,
            CancellationToken token = default);

        UniTask<Result<Void>> SelectItem(Item item, CancellationToken token = default);
        UniTask<Result<List<Item>>> GetUserSelectedItems(CancellationToken token = default);



        UniTask<Result<List<Item>>> GetInventory(CancellationToken token = default);
        UniTask<Result<Void>> PurchaseItem(Item item, CancellationToken token = default);
        UniTask<Result<List<Item>>> GetUserStore(CancellationToken token = default);

        UniTask<Result<List<BaseNotification>>> GetNotifications(int page = 0, int pageSize = 30, CancellationToken token = default);
        UniTask<Result<Void>> MarkNotificationAsRead(BaseNotification notification, CancellationToken token = default);
        UniTask<Result<Void>> RemoveNotification(BaseNotification notification, CancellationToken token = default);
        UniTask<Result<int>> LikeComment(Comment comment, CancellationToken token = default);
        UniTask<Result<int>> DislikeComment(Comment comment, CancellationToken token = default);


        UniTask<Result<List<ChatMessage>>> GetChatPage(User receiver, int startIndex = 0,
            int pageSize = 10, CancellationToken token = default);
        UniTask<Result<Void>> SendChatMessage(User receiver, string message, CancellationToken token = default);


        UniTask<Result<IEnumerable<Post>>> GetUserFeed(int startIndex = 0, int pageSize = 10, CancellationToken token = default);
        UniTask<Result<IEnumerable<Post>>> GetPostsByUser(User user, int startIndex = 0, int pageSize = 10, CancellationToken token = default);
        UniTask<Result<Post>> GetPostById(string id, CancellationToken token = default);
        UniTask<Result<Post>> CreatePost(string content, EPostType postType, CancellationToken token = default);
        UniTask<Result<Void>> HidePost(Post post, CancellationToken token = default);
        UniTask<Result<int>> LikePost(Post post, CancellationToken token = default);
        UniTask<Result<IEnumerable<Post>>> GetPostsLikedByUser(int pageStart = 0, int pageSize = 10, CancellationToken token = default);
        UniTask<Result<Comment>> CommentOnPost(Post post, string commentContent, CancellationToken token = default);

        UniTask<Result<Tournament>> CreateTournament(TimeSpan lifetime, int maxWinnerCount, CancellationToken token = default);

        UniTask<Result<Void>> FilterUser(CancellationToken token = default);

        UniTask<Result<Room>> CreateRoom(string roomName, int maxMemberCount = 30, bool isPrivate = true,
            TimeSpan lifetime = new TimeSpan(),
            int logoIndex = 0, CancellationToken token = default);

        UniTask<Result<List<Room>>> GetUserRooms(CancellationToken token = default);
        UniTask<Result<List<Room>>> GetRooms(string name = "", int kingdom = -1, CancellationToken token = default);
        UniTask<Result<Void>> LeaveRoom(Room room, CancellationToken token = default);
        UniTask<Result<Void>> JoinRoom(Room room, CancellationToken token = default);
        UniTask<Result<Void>> DeleteRoom(Room room, CancellationToken token = default);

        UniTask<Result<List<Tournament>>> GetTournaments(int page = 0, int pageSize = 10,
            CancellationToken token = default);

        UniTask<Result<Void>> UpdateMemberRole(Room room, User user, CancellationToken token = default);
        UniTask<Result<Void>> DeleteChatMessage(ChatMessage message, CancellationToken token = default);
        UniTask<Result<User>> CreateUser(CreateUserEntity createUserEntity, CancellationToken token = default);

        UniTask<Result<Void>> UpdateCustomizations(CharacterAvatarData avatarData, CancellationToken token = default);
        UniTask<Result<CharacterAvatarData>> GetCustomizationAvatar(string userId,CancellationToken token = default);

        UniTask<Result<int>> RemoveLikePost(Post post, CancellationToken token = default);
        UniTask<Result<Void>> DeletePost(Post post, CancellationToken token = default);

        /// <summary>
        /// Downloads a texture from a given URL and returns a Result containing the texture if successful.
        /// </summary>
        /// <param name="photoUrl">URL of the texture to download</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Result containing the downloaded texture or an error message</returns>
        UniTask<Result<Texture2D>> DownloadTexture(string photoUrl, CancellationToken token = default);

        UniTask<Result<Void>> AdminUpdateNItems(List<Item> items, CancellationToken token = default);
    }

    public abstract class BaseBalootApiCommand<T> : BaseApiCommand<T>
    {
        protected IApiHandler ApiHandler;

        public BaseBalootApiCommand()
        {
            ApiHandler = BalootLifetimeScope.Resolver.Resolve<IApiHandler>();
        }
    }


    public class BalootApiHandler : IApiHandler
    {
        private User _signedInUser;
        private readonly IApiRequest _apiRequest;
        private readonly IAuthService _authService;

        // Repeated endpoint constants
        private const string UserEndpoint = "users";
        private const string UsersEndpoint = "users";
        private const string FriendsSendFriendRequestEndpoint = "relations/create";
        private const string FriendsAcceptFriendRequestEndpoint = "relations/accept-friend-request";
        private const string FriendsUnfriendEndpoint = "relations";
        private const string FollowingsEndpoint = "followings";
        private const string FollowingsUnfollowEndpoint = "followings/unfollow";
        private const string PlayerSelectedItemsEndpoint = "player-selected-items/player";
        private const string SelectedItemsEndpoint = "player-selected-items";
        private const string InventoryEndpoint = "inventory";
        private const string ItemBuyForEndpoint = "item/buy-for";
        private const string ItemsEndpoint = "item";
        private const string NotificationsEndpoint = "notifications";
        private const string CommentsEndpoint = "comments";
        private const string MessageEndpoint = "message";
        private const string PostsEndpoint = "posts";
        private const string GoldenPostEndpoint = "posts/golden";
        private const string RoomEndpoint = "room";
        private const string AddMemberToRoomEndpoint = "room/add-member";
        private const string TournamentEndpoint = "tournament";
        private const string AuthEndpoint = "auth";
        private const string LoginTokenEndpoint = "auth/login-token";
        private const string LoginEndpoint = "auth/login";
        private const string RegisterEndpoint = "auth/register";
        private const string RegisterTokenEndpoint = "auth/register-token";
        private string UpdateMemberRoleEndpoint(int roomId, int memberId) => $"room/{roomId}/member/{memberId}/role";
        private string GetUserRoomsEndpoint() => $"room/user/{_signedInUser.Id}";

        // A cache for users, keyed by userId.
        private readonly Dictionary<string, (User user, DateTime cachedAt)> _userCache = new();

        // Cache expiration time for users (e.g., 10 minutes)
        private readonly TimeSpan _userCacheExpiration = TimeSpan.FromMinutes(10);

        // Optionally, you can also cache posts for a user. This key might combine userId and pagination.
        private readonly Dictionary<string, (IEnumerable<Post> posts, DateTime cachedAt)> _postsCache = new();

        // Cache expiration time for posts (e.g., 5 minutes)
        private readonly TimeSpan _postsCacheExpiration = TimeSpan.FromMinutes(5);
        private Dictionary<string, (Texture2D picture, DateTime cachedAt)> _cachedPhotos = new();

        [Inject]
        public BalootApiHandler(IApiRequest apiRequest, IAuthService authService)
        {
            _apiRequest = apiRequest;
            _authService = authService;
            ApiEventBus<OnUserLogin>.Register(OnUserLogin);
        }

        ~BalootApiHandler()
        {
            ApiEventBus<OnUserLogin>.Deregister(OnUserLogin);
        }

        public void OnUserLogin(OnUserLogin obj)
        {
            _signedInUser = obj.User;
        }

        public async UniTask<Result<User>> SignInWithToken(string loginToken, CancellationToken token = default)
        {
            var response = await _apiRequest.PostRequest($"{LoginTokenEndpoint}/{loginToken}", cancellationToken:token);
            if (response.IsSuccess)
            {
                var loginResultDto = JsonConvert.DeserializeObject<LoginResultDto>(response.Value);
                var userDto = loginResultDto.User;
                var user = userDto.Adapt<User>();
                _signedInUser = user;
                var profilePic = await DownloadTexture(userDto.PhotoUrl, token);
                if (profilePic)
                {
                    user.ProfilePic = profilePic.Value;
                }
                // Set JWT token for future requests
                _authService.SetToken(loginResultDto.AccessToken);
                ApiEventBus<OnUserLogin>.Raise(new OnUserLogin(user));
                return Result<User>.Success(user);
            }
            return Result<User>.Failure(response.ErrorMessage);
        }


        public async UniTask<Result<User>> SignInWithPassword(LoginDto loginDto, CancellationToken token = default)
        {
            var response = await _apiRequest.PostRequest($"{LoginEndpoint}", JsonConvert.SerializeObject(loginDto), token);
            if (response.IsSuccess)
            {
                var loginResultDto = JsonConvert.DeserializeObject<LoginResultDto>(response.Value);
                var userDto = loginResultDto.User;
                var user = userDto.Adapt<User>();
                var profilePic = await DownloadTexture(userDto.PhotoUrl, token);
                if (profilePic)
                {
                    user.ProfilePic = profilePic.Value;
                }
                _signedInUser = user;

                // Set JWT token for future requests
                _authService.SetToken(loginResultDto.AccessToken);
                ApiEventBus<OnUserLogin>.Raise(new OnUserLogin(user));
                return Result<User>.Success(user);
            }
            return Result<User>.Failure(response.ErrorMessage);
        }

        public async UniTask<Result<User>> Register(RegisterDto registerDto, CancellationToken token = default)
        {
            var response = await _apiRequest.PostRequest($"{RegisterEndpoint}", JsonConvert.SerializeObject(registerDto), token);
            if (response.IsSuccess)
            {
                var loginResultDto = JsonConvert.DeserializeObject<LoginResultDto>(response.Value);
                var userDto = loginResultDto.User;
                var user = userDto.Adapt<User>();
                _signedInUser = user;

                // Set JWT token for future requests
                _authService.SetToken(loginResultDto.AccessToken);
                ApiEventBus<OnUserLogin>.Raise(new OnUserLogin(user));
                return Result<User>.Success(user);
            }
            return Result<User>.Failure(response.ErrorMessage);
        }

        public async UniTask<Result<User>> RegisterWithToken(string loginToken, RegisterDto registerDto, CancellationToken token = default)
        {
            var response = await _apiRequest.PostRequest($"{RegisterTokenEndpoint}/{loginToken}", JsonConvert.SerializeObject(registerDto), token);
            if (response.IsSuccess)
            {
                var loginResultDto = JsonConvert.DeserializeObject<LoginResultDto>(response.Value);
                var userDto = loginResultDto.User;
                var user = userDto.Adapt<User>();

                _signedInUser = user;

                // Set JWT token for future requests
                _authService.SetToken(loginResultDto.AccessToken);
                ApiEventBus<OnUserLogin>.Raise(new OnUserLogin(user));
                // return await GetUser(userDto.Id.ToString(), token);
                return Result<User>.Success(user);
            }
            return Result<User>.Failure(response.ErrorMessage);
        }

        public async UniTask<Result<User>> GetUser(string userId, bool invalidateCache = false, CancellationToken token = default)
        {
            // Check if the user is already in the cache and still valid
            if (!invalidateCache)
            {
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
            }
            var response = await _apiRequest.GetRequest($"{UserEndpoint}/{userId}", token);
            if (response.IsSuccess)
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                var userDto = JsonConvert.DeserializeObject<UserDto>(response.Value, jsonSettings);
                var user = userDto.Adapt<User>();
                var profilePic = await DownloadTexture(userDto.PhotoUrl, token);
                if (profilePic)
                {
                    user.ProfilePic = profilePic.Value;
                }
                _userCache[userId] = (user, DateTime.UtcNow);
                return Result<User>.Success(user);
            }
            return Result<User>.Failure(response.ErrorMessage);
        }

        public async UniTask<Result<List<User>>> GetUsers(int pageStart = 0, int pageSize = 10, string name = "", CancellationToken token = default)
        {
            var response = await _apiRequest.GetRequest($"users/list-for?page_start={pageStart}&page_size={pageSize}&name={name}");
            if (response.IsSuccess)
            {
                var settings = new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                };
                var usersDto = JsonConvert.DeserializeObject<ArrayDto<UserDto>>(response.Value, settings);
                var users = usersDto.Data.Adapt<List<User>>();
                var downloadTasks = usersDto
                    .Data
                    .Select((dto, idx) => DownloadAndAssignTexture(dto.PhotoUrl, users[idx], token))
                    .ToArray();
                await UniTask.WhenAll(downloadTasks);

                return Result<List<User>>.Success(users);
            }
            return Result<List<User>>.Failure(response.ErrorMessage);
        }

        private async UniTask DownloadAndAssignTexture(string photoUrl, User user, CancellationToken token)
        {
            if (string.IsNullOrEmpty(photoUrl))
                return;

            UnityWebRequest www = UnityWebRequestTexture.GetTexture(photoUrl);

            try
            {
                await www.SendWebRequest().ToUniTask(cancellationToken: token);

                if (www.result == UnityWebRequest.Result.Success)
                {
                    var tex = DownloadHandlerTexture.GetContent(www);
                    user.ProfilePic = tex;
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
            }
            finally
            {
                www.Dispose();
            }
        }

        /// <summary>
        /// Downloads a texture from a given URL and returns a Result containing the texture if successful.
        /// </summary>
        /// <param name="photoUrl">URL of the texture to download</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Result containing the downloaded texture or an error message</returns>
        public async UniTask<Result<Texture2D>> DownloadTexture(string photoUrl, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(photoUrl))
                return Result<Texture2D>.Failure(EResultError.NullValue, "Photo URL is null or empty");

            if (_cachedPhotos.TryGetValue(photoUrl, out var cacheEntry))
            {
                if (DateTime.UtcNow - cacheEntry.cachedAt < _userCacheExpiration)
                {
                    return Result<Texture2D>.Success(cacheEntry.picture);
                }
                else
                {
                    // Optionally remove expired entry
                    _userCache.Remove(photoUrl);
                }
            }

            UnityWebRequest www = UnityWebRequestTexture.GetTexture(photoUrl);

            try
            {
                await www.SendWebRequest().ToUniTask(cancellationToken: token);

                if (www.result == UnityWebRequest.Result.Success)
                {
                    var tex = DownloadHandlerTexture.GetContent(www);
                    return Result<Texture2D>.Success(tex);
                }
                else
                {
                    return Result<Texture2D>.Failure(EResultError.ServerError, $"Failed to download texture: {www.error}");
                }
            }
            catch (OperationCanceledException)
            {
                return Result<Texture2D>.Failure(EResultError.Unknown, "Texture download was cancelled");
            }
            catch (Exception ex)
            {
                return Result<Texture2D>.Failure(EResultError.Unknown, $"Exception occurred while downloading texture: {ex.Message}");
            }
            finally
            {
                www.Dispose();
            }
        }

        public async UniTask<Result<Void>> AdminUpdateNItems(List<Item> items, CancellationToken token = default)
        {
            var result = await _apiRequest.PatchRequest($"{ItemsEndpoint}", JsonConvert.SerializeObject(items.Adapt<List<ItemDto>>()), token);
            return result;
        }


        public async UniTask<Result<List<User>>> GetUsersByIds(IEnumerable<string> ids, int pageStart = 0, int pageSize = 10, CancellationToken token = default)
        {
            var idsToInt = ids.Select(int.Parse);
            var idsDto = new IdsDto()
            {
                Ids = idsToInt.ToArray()
            };
            var idsJson = JsonConvert.SerializeObject(idsDto);
            var usersResult = await _apiRequest.PostRequest($"{UsersEndpoint}/batch", idsJson, token);
            if (usersResult)
            {
                var usersDtos = JsonConvert.DeserializeObject<List<UserDto>>(usersResult.Value);
                var users = usersDtos.Adapt<List<User>>();
                return Result<List<User>>.Success(users);
            }
            return Result<List<User>>.Failure(usersResult.ErrorMessage);
        }

        public async UniTask<Result<List<User>>> GetFriendList(CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest("relations?type=friend", token);
            if (result)
            {
                var relationDtos = JsonConvert.DeserializeObject<ArrayDto<RelationDto>>(result.Value);
                var users = relationDtos.Data.Where(x => x.Accepted).Adapt<List<UserDto>>();
                for (var i = 0; i < users.Count; i++)
                {
                    var user = users[i];
                    var dto = user;
                    dto.IsFriend = true;
                    users[i] = dto;
                }

                var userEntities = users.Adapt<List<User>>();
                return Result<List<User>>.Success(userEntities);
            }
            return Result<List<User>>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<List<User>>> GetFriendRequests(int page = 0, int pageSize = 30, CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest($"relations/requests??page={page}&page_size={pageSize}", token);
            if (result)
            {
                var relationDots = JsonConvert.DeserializeObject<ArrayDto<RelationDto>>(result.Value);
                var users = relationDots.Data.Where(x => x.Accepted == false).Adapt<List<UserDto>>();
                var userEntities = users.Adapt<List<User>>();
                return Result<List<User>>.Success(userEntities);
            }
            return Result<List<User>>.Failure(result.ErrorMessage);
        }


        public async UniTask<Result<Void>> SendFriendRequest(User user, CancellationToken token = default)
        {
            var json = JsonConvert.SerializeObject(new SendFriendRequestDto(_signedInUser.Id, int.Parse(user.Id)));
            return (await _apiRequest.PostRequest(FriendsSendFriendRequestEndpoint, json, token)).ToResult();
        }

        public async UniTask<Result<Void>> AcceptFriendRequest(User user, CancellationToken token = default)
        {
            var json = JsonConvert.SerializeObject(new SendFriendRequestDto(_signedInUser.Id, int.Parse(user.Id)));
            return (await _apiRequest.PostRequest(FriendsAcceptFriendRequestEndpoint, json, token)).ToResult();
        }

        public async UniTask<Result<Void>> FollowUser(User user, CancellationToken token = default)
        {
            var json = JsonConvert.SerializeObject(new SendFriendRequestDto(_signedInUser.Id, int.Parse(user.Id)));
            return (await _apiRequest.PostRequest(FollowingsEndpoint, json, token)).ToResult();
        }

        public async UniTask<Result<Void>> UnfollowUser(User user, CancellationToken token = default)
        {
            return (await _apiRequest.DeleteRequest($"{FollowingsUnfollowEndpoint}/{user.Id}", cancellationToken: token)).ToResult();
        }

        public async UniTask<Result<Void>> RemoveFriend(User user, CancellationToken token = default)
        {
            if (user is null) return Result<Void>.Failure(EResultError.NullValue, "User passed in is null.");
            var result = await _apiRequest.DeleteRequest($"{FriendsUnfriendEndpoint}/{user.Id}", token);
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
            if (NullParameterCheck<Void>(picture, out var nullResult)) return nullResult;
            var pictureFormItem = new FormItem("image", SerializationUtilities.SerializeToByteArr(picture), EFormItemType.ByteArray);
            var result = await _apiRequest.PatchRequestForm($"{UsersEndpoint}/{_signedInUser.Id}/photo", token, formItems: pictureFormItem);
            return result.ToResult();
        }
        public async UniTask<Result<Void>> UpdateStatus(string status, CancellationToken token = default)
        {
            var statusFormItem = new FormItem("status", status, EFormItemType.StringValue);
            var result = await _apiRequest.PatchRequestForm($"{UsersEndpoint}/{_signedInUser.Id}", token, statusFormItem);
            return result.ToResult();
        }

        public async UniTask<Result<Void>> UpdatePlayerGameState(string userId, int coins, int points, bool isWinner, bool isRanking, CancellationToken token = default)
        {
            var updatePlayerGameStateDto = new UpdatePlayerGameStateDto()
            {
                Points = points,
                Coins = coins,
                IsWinner = isWinner,
                IsRanking = isRanking
            };
            return await _apiRequest.PatchRequest($"{UsersEndpoint}/update-match-points/{userId}",
                JsonConvert.SerializeObject(updatePlayerGameStateDto)
                ,token
            );
        }

        public async UniTask<Result<Void>> SelectItem(Item item, CancellationToken token = default)
        {
            if (NullParameterCheck<Void>(item, out var nullResult)) return nullResult;
            var selectItemDto = new ItemSelectDto(_signedInUser.Id, item.Id);
            var result = await _apiRequest.PostRequest(SelectedItemsEndpoint, JsonConvert.SerializeObject(selectItemDto), token);
            return result;
        }

        public async UniTask<Result<List<Item>>> GetUserSelectedItems(CancellationToken token = default)
        {
            var response = await _apiRequest.GetRequest($"{PlayerSelectedItemsEndpoint}/{_signedInUser.Id}", token);
            if (response.IsSuccess)
            {
                var itemDtos = JsonConvert.DeserializeObject<ArrayDto<SelectedItemsDto>>(response.Value).Data;
                var items = itemDtos.Adapt<List<Item>>();
                return Result<List<Item>>.Success(items);
            }
            return Result<List<Item>>.Failure(response.ErrorMessage);
        }

        public async UniTask<Result<List<Item>>> GetInventory(CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest($"{InventoryEndpoint}/my-inventory", token);
            if (result.IsSuccess)
            {
                var json = JObject.Parse(result.Value);
                var itemInventoryDtos = JsonConvert.DeserializeObject<List<InventoryItemDto>>(json["items"].ToString());
                var itemEntities = itemInventoryDtos.Adapt<List<Item>>();
                _signedInUser.Inventory = itemEntities;
                return Result<List<Item>>.Success(itemEntities);
            }
            return Result<List<Item>>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<Void>> PurchaseItem(Item item, CancellationToken token = default)
        {
            if (NullParameterCheck<Void>(item, out var nullResult)) return nullResult;
            var itemPurchaseDto = new ItemPurchaseDto(int.Parse(item.Id), 1);
            var result = await _apiRequest.PostRequest($"{ItemBuyForEndpoint}/{_signedInUser.Id}", JsonConvert.SerializeObject(itemPurchaseDto), token);
            return result.ToResult();
        }

        public async UniTask<Result<List<Item>>> GetUserStore(CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest($"{ItemsEndpoint}?page=0&page_size=30", token);
            if (!result.IsSuccess) return Result<List<Item>>.Failure(result.ErrorMessage);
            var itemDtos = JsonConvert.DeserializeObject<ArrayDto<ItemDto>>(result.Value);
            var itemEntities = itemDtos.Data.Adapt<List<Item>>();
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
            var result = await _apiRequest.GetRequest($"{NotificationsEndpoint}/user?page={page}&page_size={pageSize}");
            if (result.IsSuccess)
            {
                var notificationsDto = JsonConvert.DeserializeObject<ArrayDto<NotificationDto>>(result.Value).Data;
                var notifications = notificationsDto.Adapt<List<BaseNotification>>();
                foreach (var notification in notifications)
                {
                    notification.Receiver = _signedInUser;
                }
                return Result<List<BaseNotification>>.Success(notifications.Select(BaseNotification.CreateNotification).ToList());
            }
            return Result<List<BaseNotification>>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<Void>> MarkNotificationAsRead(BaseNotification notification, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Void>> RemoveNotification(BaseNotification notification, CancellationToken token = default)
        {
            var result = await _apiRequest.DeleteRequest($"{NotificationsEndpoint}/{notification.Id}", token);
            return result;
        }

        public async UniTask<Result<int>> LikeComment(Comment comment, CancellationToken token = default)
        {
            var result = await _apiRequest.PostRequest($"{CommentsEndpoint}/{comment.Id}/like", cancellationToken: token);
            if (result.IsSuccess)
            {
                comment.LikeCount = JsonConvert.DeserializeObject<int>(result.Value);
                return Result<int>.Success(comment.LikeCount);
            }
            return Result<int>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<int>> DislikeComment(Comment comment, CancellationToken token = default)
        {
            var result = await _apiRequest.PostRequest($"{CommentsEndpoint}/{comment.Id}/dislike", cancellationToken: token);
            if (result.IsSuccess)
            {
                comment.LikeCount = JsonConvert.DeserializeObject<int>(result.Value);
                return Result<int>.Success(comment.LikeCount);
            }
            return Result<int>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<List<ChatMessage>>> GetChatPage(User receiver, int startIndex = 0, int pageSize = 10, CancellationToken token = default)
        {
            var result = await _apiRequest.GetRequest($"{MessageEndpoint}/chat/{receiver.Id}?page={startIndex}&page_size={pageSize}", token);
            if (result.IsSuccess)
            {
                var chatDto = JsonConvert.DeserializeObject<ArrayDto<ChatMessageDto>>(result.Value).Data;
                var chat = chatDto.Adapt<List<ChatMessage>>();
                for (int i = 0; i < chat.Count; ++i)
                {
                    var dto = chatDto[i];
                    var entity = chat[i];
                    entity.Sender = dto.User1Id == _signedInUser.Id ? _signedInUser : receiver;
                    entity.Receiver = dto.User2Id == _signedInUser.Id ? receiver : _signedInUser;
                    chat[i] = entity;
                }
                return Result<List<ChatMessage>>.Success(chat);
            }
            return Result<List<ChatMessage>>.Failure(result.ErrorMessage);
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
        public async UniTask<Result<Void>> DeleteChatMessage(ChatMessage message, CancellationToken token = default)
        {
            return await _apiRequest.DeleteRequest($"{MessageEndpoint}/{message.Id}", token);
        }

        public async UniTask<Result<User>> CreateUser(CreateUserEntity createUserEntity, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Void>> UpdateCustomizations(CharacterAvatarData avatarData, CancellationToken token = default)
        {
            var customizationItems = avatarData.CustomizationItemsDictionary.Select(x => x.Value).ToList();
            var customizationItemsDto = customizationItems.Adapt<List<UpdateCustomizationItemDto>>();
            var json = JsonConvert.SerializeObject(customizationItemsDto);
            var result = await _apiRequest.PatchRequest($"customization", json, token);
            return result.ToVoidResult();
        }

        public async UniTask<Result<CharacterAvatarData>> GetCustomizationAvatar(string userId,CancellationToken token = default)
        {
            var response = await _apiRequest.GetRequest($"customization/user/{userId}", token);
            if (response.IsSuccess)
            {
                var customizationItemsDto = JsonConvert.DeserializeObject<ArrayDto<CustomizationItemDto>>(response.Value);
                var customizationItems =
                    customizationItemsDto.Data.Adapt<List<ColorCustomizationItem>>();
                var avatarData = new CharacterAvatarData(customizationItems);
                return Result<CharacterAvatarData>.Success(avatarData);
            }

            return Result<CharacterAvatarData>.Failure(response.ErrorMessage);
        }

        public async UniTask<Result<IEnumerable<Post>>> GetUserFeed(int startIndex = 0, int pageSize = 10, CancellationToken token = default)
        {
            var response = await _apiRequest.GetRequest($"{PostsEndpoint}/my-posts?page={startIndex}&page_size={pageSize}", token);
            if (response.IsSuccess)
            {
                var postDtos = JsonConvert.DeserializeObject<ArrayDto<PostDto>>(response.Value);
                var postEntities = postDtos.Data.Adapt<List<Post>>();
                for (var i = 0; i < postEntities.Count; i++)
                {
                    var dto = postDtos.Data[i];
                    var entity = postEntities[i];
                    entity.User = new()
                    {
                        Id = dto.UserId
                    };

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

        public async UniTask<Result<Post>> GetPostById(string id, CancellationToken token = default)
        {
            var response = await _apiRequest.GetRequest($"{PostsEndpoint}/{id}");
            if (response.IsSuccess)
            {
                var post = JsonConvert.DeserializeObject<PostDto>(response.Value);
                var postEntity = post.Adapt<Post>();
                for (var i = 0; i < postEntity.Comments.Count; i++)
                {
                    var commentDto = post.Comments[i];
                    var comment = postEntity.Comments[i];
                    comment.User = new()
                    {
                        Id = commentDto.UserId
                    };
                }

                var user = await GetUser(post.UserId, token:token);
                if (user)
                {
                    postEntity.User = user.Value;
                }
                else
                {
                    postEntity.User = new()
                    {
                        Id = post.Id
                    };
                }

                return Result<Post>.Success(postEntity);
            }
            return Result<Post>.Failure(response.ErrorMessage);
        }

        public async UniTask<Result<Post>> CreatePost(string content, EPostType postType, CancellationToken token = default)
        {
            var postCreationDto = new PostCreationDTO
            {
                Content = content,
            };
            if (postType == EPostType.Free)
            {
                var postResult = await _apiRequest.PostRequest(PostsEndpoint, JsonConvert.SerializeObject(postCreationDto), token);
                if (postResult)
                {
                    var postDto = JsonConvert.DeserializeObject<PostDto>(postResult.Value);
                    var post = postDto.Adapt<Post>();
                    post.User = new User()
                    {
                        Id = postDto.UserId,
                    };
                    return Result<Post>.Success(post);
                }
                else
                {
                    return Result<Post>.Failure(postResult.ErrorMessage);
                }
            }
            else
            {
                var postResult = await _apiRequest.PostRequest(GoldenPostEndpoint, JsonConvert.SerializeObject(postCreationDto), token);
                if (postResult)
                {
                    var postDto = JsonConvert.DeserializeObject<PostDto>(postResult.Value);
                    var post = postDto.Adapt<Post>();
                    post.User = new User()
                    {
                        Id = postDto.UserId,
                    };
                    return Result<Post>.Success(post);
                }
                else
                {
                    return Result<Post>.Failure(postResult.ErrorMessage);
                }
            }
        }

        public async UniTask<Result<Void>> HidePost(Post post, CancellationToken token = default)
        {
            var result = await _apiRequest.PostRequest($"{PostsEndpoint}/{post.Id}/hide/{_signedInUser.Id}", "", token);
            return result;
        }
        public async UniTask<Result<Void>> DeletePost(Post post, CancellationToken token = default)
        {
            var result = await _apiRequest.DeleteRequest($"{PostsEndpoint}/{post.Id}", token);
            return result;
        }

        public async UniTask<Result<int>> LikePost(Post post, CancellationToken token = default)
        {
            var result = await _apiRequest.PostRequest($"{PostsEndpoint}/{post.Id}/like", "", token);
            if (result.IsSuccess)
            {
                var count = int.Parse(result.Value);
                post.LikesCount = count;
                return Result<int>.Success(count);
            }
            return Result<int>.Failure(result.ErrorMessage);
        }
        public async UniTask<Result<int>> RemoveLikePost(Post post, CancellationToken token = default)
        {
            var result = await _apiRequest.PostRequest($"{PostsEndpoint}/{post.Id}/dislike", "", token);
            if (result.IsSuccess)
            {
                var count = int.Parse(result.Value);
                post.LikesCount = count;
                return Result<int>.Success(count);
            }
            return Result<int>.Failure(result.ErrorMessage);
        }
        public async UniTask<Result<IEnumerable<Post>>> GetPostsLikedByUser(int pageStart = 0, int pageSize = 10, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async UniTask<Result<Comment>> CommentOnPost(Post post, string commentContent, CancellationToken token = default)
        {
            var postCreationDto = new PostCreationDTO
            {
                Content = commentContent
            };
            var result = await _apiRequest.PostRequest($"{PostsEndpoint}/{post.Id}/comment", JsonConvert.SerializeObject(postCreationDto), token);
            if (result)
            {
                var commentDto = JsonConvert.DeserializeObject<CommentDto>(result.Value);
                var comment = commentDto.Adapt<Comment>();
                var user = await GetUser(commentDto.UserId);
                if (user)
                {
                    comment.User = user.Value;
                }
                else
                {
                    comment.User = new User()
                    {
                        Id = commentDto.UserId
                    };
                }

                return Result<Comment>.Success(comment);
            }

            return Result<Comment>.Failure(result.ErrorMessage);
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
                        await RequestWithRetry(() => GetUser(winnerId.ToString(), token:token), (v) => { winners.Add(v); },
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
                Lifetime = 9999,
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
                var roomsDto = JsonConvert.DeserializeObject<ArrayDto<RoomDto>>(result.Value);
                if (roomsDto.Data.Count == 0)
                {
                    return Result<List<Room>>.Success(new());
                }

                var rooms = roomsDto.Data.Adapt<List<Room>>();
                // var successfulRooms = new List<Room>();
                // for (int i = 0; i < roomsDto.Data.Count; ++i)
                // {
                //     var dto = roomsDto.Data[i];
                //     var room = rooms[i];
                //     await RequestWithRetry(() => GetUser(dto.OwnerId.ToString(), token:token),
                //         (v) => {
                //             room.Owner = v;
                //             successfulRooms.Add(room);
                //         }, token);
                // }

                if (rooms.Count == 0)
                {
                    return Result<List<Room>>.Failure(EResultError.PartialSuccess,
                        "Failed to parse the returned rooms.");
                }
                return Result<List<Room>>.Success(rooms);
            }
            return Result<List<Room>>.Failure(result.ErrorMessage);
        }
        public async UniTask<Result<List<Room>>> GetRooms(string name = "", int kingdom = -1, CancellationToken token = default)
        {
            var request = $"{RoomEndpoint}?name={name}";
            if (kingdom >= 0)
            {
                request = $"{RoomEndpoint}?name={name}&logo={kingdom}";
            }
            var result = await _apiRequest.GetRequest(request, token);
            if (result.IsSuccess)
            {
                var roomsDto = JsonConvert.DeserializeObject<ArrayDto<RoomDto>>(result.Value);
                if (roomsDto.Data.Count == 0)
                {
                    return Result<List<Room>>.Success(new());
                }

                var rooms = roomsDto.Data.Adapt<List<Room>>();
                return Result<List<Room>>.Success(rooms);
            }
            return Result<List<Room>>.Failure(result.ErrorMessage);
        }

        public async UniTask<Result<Void>> LeaveRoom(Room room, CancellationToken token = default)
        {
            var request = await _apiRequest.DeleteRequest($"{RoomEndpoint}/leave/{room.Id}", token);
            return request;
        }

        public async UniTask<Result<Void>> JoinRoom(Room room, CancellationToken token = default)
        {
            var request = await _apiRequest.PostRequest($"{RoomEndpoint}/request-join/{room.Id}", cancellationToken:token);
            return request;
        }

        public async UniTask<Result<Void>> DeleteRoom(Room room, CancellationToken token = default)
        {
            var request = await _apiRequest.DeleteRequest($"{RoomEndpoint}/{room.Id}", token);
            return request;
        }


        public async UniTask<Result<Void>> UpdateMemberRole(Room room, User user, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        private static async UniTask<Result<T>> RequestWithRetry<T>(Func<UniTask<Result<T>>> requestFunc, Action<T> onRequestSuccess, CancellationToken token = default)
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
        private static bool NullParameterCheck<T>(object obj, out Result<T> result)
        {
            result = Result<T>.Failure(EResultError.NullValue, "Object is null.");
            if (obj is null)
            {
                return true;
            }
            return false;
        }


    }

    public struct OnUserLogin
    {
        public User User { get; }

        public OnUserLogin(User user)
        {
            User = user;
        }
    }
}
