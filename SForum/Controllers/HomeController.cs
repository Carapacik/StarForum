using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SForum.Data;
using SForum.Data.Models;
using SForum.Models;
using SForum.Models.Forum;
using SForum.Models.Home;
using SForum.Models.Post;

namespace SForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly IForum _forumService;
        private readonly IPost _postService;

        public HomeController(IPost postService, IForum forumService)
        {
            _postService = postService;
            _forumService = forumService;
        }

        public IActionResult Index()
        {
            var model = BuildHomeIndexModel();
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        private HomeIndexModel BuildHomeIndexModel()
        {
            var latestPosts = _postService.GetLatestPosts(5);
            var posts = latestPosts.Select(post => new PostListingModel
            {
                Id = post.Id,
                Title = post.Title,
                AuthorId = post.User.Id,
                Author = post.User.UserName,
                AuthorRating = post.User.Rating,
                DatePosted = post.Created.ToString(CultureInfo.InvariantCulture),
                RepliesCount = post.Replies.Count(),
                Forum = GetForumListingForPost(post)
            });

            var popularForums = _forumService.GetPopularForums(5);
            var forums = popularForums
                .Select(forum => new ForumListingModel
                {
                    Id = forum.Id,
                    Name = forum.Title,
                    Description = forum.Description,
                    NumberOfPosts = forum.Posts?.Count() ?? 0,
                    NumberOfUsers = _forumService.GetActiveUsers(forum.Id).Count(),
                    ImageUrl = forum.ImageUrl,
                    HasRecentPosts = _forumService.HasRecentPost(forum.Id)
                });

            return new HomeIndexModel
            {
                LatestPosts = posts,
                PopularForums = forums,
                SearchQuery = ""
            };
        }

        private static ForumListingModel GetForumListingForPost(Post post)
        {
            var forum = post.Forum;
            return new ForumListingModel
            {
                Name = forum.Title,
                ImageUrl = forum.ImageUrl,
                Id = forum.Id
            };
        }
    }
}