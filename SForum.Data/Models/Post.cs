using System;
using System.Collections.Generic;

namespace SForum.Data.Models
{
    public sealed class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public bool IsArchived { get; set; }
        public ApplicationUser User { get; set; }
        public IEnumerable<PostReply> Replies { get; set; }
        public Forum Forum { get; set; }
    }
}