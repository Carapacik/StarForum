using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SForum.Models.Reply
{
    public class EditPostReplyModel
    {
        public int PostId { get; set; }
        public int ReplyId { get; set; }
        public string AuthorName { get; set; }
        public string Content { get; set; }
        public string PostTitle { get; set; }
    }
}
