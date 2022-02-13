using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using StarForum.Infrastructure.Entities;
using StarForum.Infrastructure.Services;
using StarForum.WebApi.Models.Forum;
using StarForum.WebApi.Models.Post;
using StarForum.WebApi.Models.Search;

namespace StarForum.WebApi.Controllers;

public class SearchController : Controller
{
    private readonly IPost _postService;

    public SearchController(IPost postService)
    {
        _postService = postService;
    }

    public IActionResult Results(string searchQuery)
    {
        if (searchQuery == null) return RedirectToAction("Index", "Home");
        var posts = _postService.GetFilteredPosts(searchQuery);
        var postListings = posts.Select(post => new PostListingModel
        {
            Id = post.Id,
            Forum = BuildForumListing(post),
            Author = post.User.UserName,
            AuthorId = post.User.Id,
            AuthorRating = post.User.Rating,
            Title = post.Title,
            DatePosted = post.Created.ToString(CultureInfo.InvariantCulture),
            RepliesCount = post.Replies.Count()
        }).OrderByDescending(post => post.DatePosted);
        var areNoResults = !string.IsNullOrEmpty(searchQuery) && !posts.Any();
        var model = new SearchResultModel
        {
            EmptySearchResults = areNoResults,
            Posts = postListings,
            SearchQuery = searchQuery
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult Search(string searchQuery)
    {
        return RedirectToAction("Results", new { searchQuery });
    }

    private ForumListingModel BuildForumListing(Post post)
    {
        var forum = post.Forum;
        return new ForumListingModel
        {
            Id = forum.Id,
            ImageUrl = forum.ImageUrl,
            Name = forum.Title,
            Description = forum.Description
        };
    }
}