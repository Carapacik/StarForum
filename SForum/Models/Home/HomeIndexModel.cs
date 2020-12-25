using System.Collections.Generic;
using SForum.Models.Forum;
using SForum.Models.Post;

namespace SForum.Models.Home
{
    public class HomeIndexModel
    {
        public string SearchQuery { get; set; }
        public IEnumerable<PostListingModel> LatestPosts { get; set; }

        public IEnumerable<ForumListingModel> PopularForums { get; set; }
    }
}