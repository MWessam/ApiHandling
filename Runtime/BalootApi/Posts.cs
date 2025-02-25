using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BalootApi
{


    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime PostCreationTime { get; set; }
        public EPostType PostType { get; set; }
        public User User { get; set; }
        public List<Comment> Comments;
        public int LikesCount;
        public int CommentsCount => Comments.Count;
        public bool IsLiked;
    }

    public enum EPostType
    {
        Normal,
        Vip
    }


    public class Comment
    {
        public int Id;
        public string Content;
        public DateTime TimeStamp;
        public User UserId;
        public int LikeCount;
        public bool IsLiked;
    }
}