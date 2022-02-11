using StarForum.WebApi.Models.Forum;
using StarForum.WebApi.Models.Post;

namespace StarForum.WebApi.Models.Home;

public class HomeIndexModel
{
    public string SearchQuery { get; set; }
    public IEnumerable<PostListingModel> LatestPosts { get; set; }
    public IEnumerable<ForumListingModel> PopularForums { get; set; }
}