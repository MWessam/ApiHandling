using System;
using Newtonsoft.Json;
namespace BalootApi
{




    public class BaseNotification
    {
        public string Id;
        public User Receiver;
        public User Sender;
        public Comment Comment;
        public Post Post;
        public ENotificationType Type;
        public bool IsRead;
        public string Content;
        public DateTime Timestamp;
        public BaseNotification(){}
        public BaseNotification(BaseNotification notification)
        {
            
            Id = notification.Id;
            Receiver = notification.Receiver;
            Sender = notification.Sender;
            Type = notification.Type;
            IsRead = notification.IsRead;
            Content = notification.Content;
            Timestamp = notification.Timestamp;
            Comment = notification.Comment;
            Post = notification.Post;
        }

        public static BaseNotification CreateNotification(BaseNotification baseNotification)
        {
            switch (baseNotification.Type)
            {
                case ENotificationType.System:
                    return new SystemNotification(baseNotification);
                case ENotificationType.PostLike:
                    return new PostLikeNotification(baseNotification);
                case ENotificationType.Comment:
                    return new PostCommentNotification(baseNotification);
                case ENotificationType.CommentLike:
                    return new CommentLikeNotification(baseNotification);
                case ENotificationType.Message:
                    return new ChatNotification(baseNotification);
                case ENotificationType.FriendRequest:
                    return new FriendRequestNotification(baseNotification);
                case ENotificationType.FriendAccepted:
                    return new FriendAcceptedNotification(baseNotification);
                case ENotificationType.Follow:
                    return new FollowNotification(baseNotification);
                default:
                    return baseNotification;
            }
        }
    }

    public enum ENotificationType
    {
        None = 0,
        System = 1,
        FriendRequest = 2,
        Message = 3,
        PostLike = 4,
        Comment = 5,
        CommentLike = 6,
        FriendAccepted = 7,
        Follow = 8,
    }
    [Serializable]
    public class FriendRequestNotification : BaseNotification
    {
        public FriendRequestNotification() {}
        public FriendRequestNotification(BaseNotification notification) : base(notification)
        {
        }
    }
    [Serializable]
    public class FriendAcceptedNotification : BaseNotification
    {
        public FriendAcceptedNotification(){}
        public FriendAcceptedNotification(BaseNotification notification) : base(notification)
        {
        }
    }
    [Serializable]
    public class FollowNotification : BaseNotification
    {
        public FollowNotification(){}
        public FollowNotification(BaseNotification notification) : base(notification)
        {
        }
    }

    [Serializable]
    public class ChatNotification : BaseNotification
    {
        public ChatNotification(){}
        public ChatNotification(BaseNotification notification) : base(notification)
        {
        }
    }
    [Serializable]
    public class SystemNotification : BaseNotification
    {
        public SystemNotification(){}
        public SystemNotification(BaseNotification notification) : base(notification)
        {
        }
    }
    [Serializable]
    public class PostLikeNotification : BaseNotification
    {
        public PostLikeNotification(){}
        public PostLikeNotification(BaseNotification notification) : base(notification)
        {
        }
    }
    [Serializable]
    public class CommentLikeNotification : BaseNotification
    {
        public CommentLikeNotification(){}
        public CommentLikeNotification(BaseNotification notification) : base(notification)
        {
        }
    }
    [Serializable]
    public class PostCommentNotification : BaseNotification
    {
        public PostCommentNotification(){}
        public PostCommentNotification(BaseNotification notification) : base(notification)
        {
        }
    }

}