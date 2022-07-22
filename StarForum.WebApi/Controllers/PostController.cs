using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StarForum.Infrastructure.Entities;
using StarForum.Infrastructure.Services;
using StarForum.WebApi.Models.Post;
using StarForum.WebApi.Models.Reply;

namespace StarForum.WebApi.Controllers;

public class PostController : Controller
{
    private static UserManager<ApplicationUser> _userManager;
    private readonly IForum _forumService;
    private readonly IPost _postService;
    private readonly IApplicationUser _userService;

    public PostController(IPost postService, IForum forumService, IApplicationUser userService,
        UserManager<ApplicationUser> userManager)
    {
        _postService = postService;
        _forumService = forumService;
        _userService = userService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int id)
    {
        var post = await _postService.GetById(id);
        if (post == null) return RedirectToAction("Error", "Home");
        var replies = BuildPostReplies(post.Replies);

        var model = new PostIndexModel
        {
            Id = post.Id,
            Title = post.Title,
            AuthorId = post.User.Id,
            AuthorName = post.User.UserName,
            AuthorImageUrl = post.User.ProfileImageUrl,
            AuthorRating = post.User.Rating,
            Created = post.Created,
            PostContent = post.Content,
            Replies = replies,
            ForumId = post.Forum.Id,
            ForumName = post.Forum.Title,
            IsAuthorAdmin = IsAuthorAdmin(post.User),
            IsPostArchived = post.IsArchived
        };

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> Archive(int id)
    {
        await _postService.Archive(id);
        return RedirectToAction("Index", new {id});
    }

    [Authorize]
    public async Task<IActionResult> UnArchive(int id)
    {
        await _postService.UnArchive(id);
        return RedirectToAction("Index", new {id});
    }

    [Authorize]
    public async Task<IActionResult> Create(int id)
    {
        var forum = await _forumService.GetById(id);
        if (forum == null) return RedirectToAction("Error", "Home");
        var model = new NewPostModel
        {
            AuthorName = User.Identity?.Name,
            Created = DateTime.Now,
            ForumName = forum.Title,
            ForumId = forum.Id,
            ForumImageUrl = forum.ImageUrl
        };
        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePost(NewPostModel model)
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);
        var post = await BuildPost(model, user);

        await _postService.Add(post);
        await _userService.UpdateUserRating(userId, typeof(Post));

        return RedirectToAction("Index", "Post", new {id = post.Id});
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _postService.GetById(id);
        if (post == null)
            return RedirectToAction("Error", "Home");
        var model = new EditPostModel
        {
            Id = post.Id,
            Content = post.Content,
            Title = post.Title,
            AuthorName = post.User.UserName
        };
        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditPost(EditPostModel model)
    {
        var post = new Post
        {
            Id = model.Id,
            Title = model.Title,
            Content = model.Content
        };
        await _postService.Edit(post);
        return RedirectToAction("Index", "Post", new {id = model.Id});
    }

    private static bool IsAuthorAdmin(ApplicationUser user)
    {
        return _userManager.GetRolesAsync(user).Result.Contains("Admin");
    }

    private async Task<Post> BuildPost(NewPostModel post, ApplicationUser user)
    {
        var forum = await _forumService.GetById(post.ForumId);
        if (forum == null) return null;
        return new Post
        {
            Title = post.Title,
            Content = post.Content,
            Created = DateTime.Now,
            User = user,
            Forum = forum
        };
    }

    private static IEnumerable<PostReplyModel> BuildPostReplies(IEnumerable<PostReply> replies)
    {
        return replies.Select(reply => new PostReplyModel
        {
            Id = reply.Id,
            AuthorId = reply.User.Id,
            AuthorName = reply.User.UserName,
            AuthorImageUrl = reply.User.ProfileImageUrl,
            AuthorRating = reply.User.Rating,
            Created = reply.Created,
            ReplyContent = reply.Content,
            IsAuthorAdmin = IsAuthorAdmin(reply.User)
        });
    }
}