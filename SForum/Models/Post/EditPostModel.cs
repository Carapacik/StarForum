using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SForum.Models.Post
{
    public class EditPostModel
    {
        public int PostId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsAdmin { get; set; }
    }
}
