using System.Collections;
using System.Threading.Tasks;
using ApiHandling.Runtime;
using BalootApi;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BalootApiHandlerTests
{
    private IApiHandler _apiHandler;
    [SetUp]
    public void Setup()
    {
        var balootApiHandler = new BalootApiHandler(new TestApiRequest("http://54.93.41.202"));
        balootApiHandler.OnUserLogin(new OnUserLogin(new User()
        {
            Id = 3 ,
            Name = "TestUser",
            Points = 100,
            PlayedMatchesCount = 10,
            WonMatchesCount = 5,
            LostMatchesCount = 5,
            RankedMatchesCount = 5,
            PlayerRank = 5,
            HighestRankReached = 5
        }));
        _apiHandler = balootApiHandler;
        MapperLayer.InitializeMappers();
    }
    
    [Test]
    public async Task GetUser_ValidUserId_ReturnsUser()
    {
        // Arrange
        var userId = "2";

        // Act
        var user = await _apiHandler.GetUser(userId);

        // Assert
        Assert.IsTrue(user.IsSuccess);
        Assert.AreEqual(userId, user.Value.Id.ToString());
    }
    
    [Test]
    public async Task GetUser_InvalidUserId_ReturnsErrorResult()
    {
        // Arrange
        var userId = "200";

        // Act
        var user = await _apiHandler.GetUser(userId);

        // Assert
        Assert.IsFalse(user.IsSuccess);
        // Assert.AreEqual(EResultError.NotFound, user.ErrorCode);
    }
    [Test]
    public async Task SendFriendRequest_ValidUserId_ReturnsSuccessResult()
    {
        // Arrange
        var user = new User()
        {
            Id = 2,
        };

        // Act
        
        var result = await _apiHandler.SendFriendRequest(user);

        // Assert
        Assert.IsTrue(result.IsSuccess);
    }
    [Test]
    public async Task SendFriendRequest_InvalidUserId_ReturnsErrorResult()
    {
        // Arrange
        var user = new User()
        {
            Id = 200,
        };

        // Act
        
        var result = await _apiHandler.SendFriendRequest(user);

        // Assert
        Assert.IsFalse(result.IsSuccess);
    }
    
    [Test]
    public async Task Follow_ValidUserId_ReturnsSuccessResult()
    {
        // Arrange
        var user = new User()
        {
            Id = 2,
        };

        // Act
        var unfollow = await _apiHandler.UnfollowUser(user);
        
        var result = await _apiHandler.FollowUser(user);

        // Assert
        Assert.IsTrue(result.IsSuccess);
    }
    [Test]
    public async Task Follow_InvalidUserId_ReturnsErrorResult()
    {
        // Arrange
        var user = new User()
        {
            Id = 200,
        };

        // Act
        
        var result = await _apiHandler.FollowUser(user);

        // Assert
        Assert.IsFalse(result.IsSuccess);
    }
    [Test]
    public async Task Unfollow_ValidUserId_ReturnsSuccessResult()
    {
        // Arrange
        var user = new User()
        {
            Id = 2,
        };

        // Act
        
        var result = await _apiHandler.UnfollowUser(user);

        // Assert
        Assert.IsTrue(result.IsSuccess);
    }
    [Test]
    public async Task Unfollow_InvalidUserId_ReturnsErrorResult()
    {
        // Arrange
        var user = new User()
        {
            Id = 200,
        };

        // Act
        
        var result = await _apiHandler.UnfollowUser(user);

        // Assert
        Assert.IsFalse(result.IsSuccess);
    }
    [Test]
    public async Task RemoveFriend_ValidUserId_ReturnsSuccessResult()
    {
        // Arrange
        var user = new User()
        {
            Id = 2,
        };

        // Act
        
        var result = await _apiHandler.RemoveFriend(user);

        // Assert
        Assert.IsTrue(result.IsSuccess);
    }
    [Test]
    public async Task RemoveFriend_InvalidUserId_ReturnsErrorResult()
    {
        // Arrange
        var user = new User()
        {
            Id = 200,
        };

        // Act
        
        var result = await _apiHandler.RemoveFriend(user);

        // Assert
        Assert.IsFalse(result.IsSuccess);
    }
    [Test]
    public async Task UpdateProfilePicture_ValidTexture_ReturnsSuccessResult()
    {
        var texture = new Texture2D(128, 128);
        var result = await _apiHandler.UpdateProfilePicture(texture);

        // Assert
        Assert.IsTrue(result.IsSuccess);
    }
    [Test]
    public async Task UpdateProfilePicture_NullTexture_ReturnsNullFailureResult()
    {
        var result = await _apiHandler.UpdateProfilePicture(null);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(EResultError.NullValue, result.ErrorCode);
    }
    [Test]
    public async Task UpdateStatus_ReturnsSuccessResult()
    {
        var result = await _apiHandler.UpdateStatus("test");
        // Assert
        Assert.IsTrue(result.IsSuccess);
    }
    [Test]
    public async Task UpdateSelectedItem_ValidItem_ReturnsSucessResult()
    {
        var selectItem = new Item()
        {
            Id = 1,
            Name = "TestItem",
            Description = "TestDescription",
            Price = 100,
            IsDefault = false
        };
        var result = await _apiHandler.SelectItem(selectItem);

        // Assert
        Assert.True(result.IsSuccess);
    }
    [Test]
    public async Task UpdateSelectedItem_InvalidItem_ReturnsFailureResult()
    {
        var selectItem = new Item()
        {
            Id = -1,
            Name = "TestItem",
            Description = "TestDescription",
            Price = 100,
            IsDefault = false
        };
        var result = await _apiHandler.SelectItem(selectItem);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(EResultError.NullValue, result.ErrorCode);
    }
    [Test]
    public async Task GetSelectedItems_ReturnsSomeItems()
    {
        var result = await _apiHandler.GetUserSelectedItems();
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.IsTrue(result.Value.Count > 0);
        Assert.IsNotNull(result.Value[0]);
        Assert.IsInstanceOf<Item>(result.Value[0]);
    }
    [Test]
    public async Task GetInventory_ReturnsSuccessfulResult()
    {
        var result = await _apiHandler.GetInventory();
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.Value.Count > 0);
        Assert.IsNotNull(result.Value);

        Assert.IsNotNull(result.Value[0]);
        Assert.IsTrue(result.Value[0].Quantity != 0);
        Assert.IsInstanceOf<Item>(result.Value[0]);
    }

    [Test]
    public async Task GetNotifications_ReturnsSuccessfulResult()
    {
        var result = await _apiHandler.GetNotifications();
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.Value.Count > 0);
        Assert.IsNotNull(result.Value);

        Assert.IsNotNull(result.Value[0]);
        Assert.IsInstanceOf<BaseNotification>(result.Value[0]);
    }
    [Test]
    public async Task LikeComment_ReturnsSuccessfulResult()
    {
        var comment = new Comment()
        {
            Content = "Test",
            Id = 1
        };
        var dislike = await _apiHandler.DislikeComment(comment);
        var result = await _apiHandler.LikeComment(comment);
        Assert.IsTrue(result.IsSuccess);
    }
    [Test]
    public async Task DislikeComment_ReturnsSuccessfulResult()
    {
        var comment = new Comment()
        {
            Content = "Test",
            Id = 1
        };
        var result = await _apiHandler.DislikeComment(comment);
        Assert.IsTrue(result.IsSuccess);
    }
    [Test]
    public async Task GetChatPage_ReturnsSuccessfulResult()
    {
        // Arrange
        var user = new User()
        {
            Id = 2,
        };
        var result = await _apiHandler.GetChatPage(user);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.Value.Count > 0);
        Assert.IsNotNull(result.Value);

        Assert.IsNotNull(result.Value[0]);
        Assert.IsInstanceOf<BaseNotification>(result.Value[0]);
    }
    [Test]
    public async Task SendChatMessage_ReturnsSuccessfulResult()
    {
        // Arrange
        var user = new User()
        {
            Id = 2,
        };
        var message = "hellooo";
        var result = await _apiHandler.SendChatMessage(user, message);
        Assert.IsTrue(result.IsSuccess);
    }
    [Test]
    public async Task DeleteChatMessage_ReturnsSuccessfulResult()
    {
        // Arrange
        var user = new User()
        {
            Id = 2,
        };
        var message = "hellooo";
        var result = await _apiHandler.DeleteChatMessage(new ChatMessageEntity()
        {
            Id = 2
        });
        Assert.IsTrue(result.IsSuccess);
    }
}
