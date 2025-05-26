using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BalootApi
{


    public class Post
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime PostCreationTime { get; set; }
        public EPostType PostType { get; set; }
        public User User { get; set; }
        public List<Comment> Comments { get; set; } = new();
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsLiked { get; set; }
    }

    public enum EPostType
    {
        Free,
        Premium
    }


    public class Comment
    {
        public string Id;
        public string Content;
        public DateTime TimeStamp;
        public User User;
        public int LikeCount;
        public bool IsLiked;
        public Post Post { get; set; }
    }
}