using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using StarForum.Infrastructure.Entities;
using StarForum.Infrastructure.Services;
using StarForum.WebApi.Models;
using StarForum.WebApi.Models.Forum;
using StarForum.WebApi.Models.Home;
using StarForum.WebApi.Models.Post;

namespace StarForum.WebApi.Controllers;

public class HomeController : Controller
{
    private readonly IForum _forumService;
    private readonly IPost _postService;

    public HomeController(IForum forumService, IPost postService)
    {
        _forumService = forumService;
        _postService = postService;
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
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
            DatePosted = post.Created.ToString(CultureInfo.CurrentCulture),
            RepliesCount = post.Replies.Count,
            Forum = GetForumListingForPost(post)
        });

        var popularForums = _forumService.GetPopularForums(5);
        var forums = popularForums
            .Select(async forum => new ForumListingModel
            {
                Id = forum.Id,
                Name = forum.Title,
                Description = forum.Description,
                NumberOfPosts = forum.Posts.Count,
                NumberOfUsers = (await _forumService.GetActiveUsers(forum.Id)).Count(),
                ImageUrl = forum.ImageUrl,
                HasRecentPosts = await _forumService.HasRecentPost(forum.Id)
            }).Select(t => t.Result);

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