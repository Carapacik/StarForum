using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StarForum.Infrastructure.Entities;
using StarForum.Infrastructure.Services;
using StarForum.WebApi.Models.Reply;

namespace StarForum.WebApi.Controllers;

[Authorize]
public class ReplyController : Controller
{
    private readonly IPost _postService;
    private readonly IPostReply _replyService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationUser _userService;

    public ReplyController(IPost postService, IApplicationUser userService, IPostReply replyService,
        UserManager<ApplicationUser> userManager)
    {
        _replyService = replyService;
        _postService = postService;
        _userService = userService;
        _userManager = userManager;
    }

    public async Task<ActionResult> Create(int id)
    {
        var post = await _postService.GetById(id);
        if (post == null) return RedirectToAction("Error", "Home");
        var user = await _userManager.FindByNameAsync(User.Identity?.Name);
        var model = new PostReplyModel
        {
            PostId = post.Id,
            PostContent = post.Content,
            PostTitle = post.Title,
            AuthorId = user.Id,
            AuthorName = User.Identity.Name,
            AuthorImageUrl = user.ProfileImageUrl,
            IsAuthorAdmin = User.IsInRole("Admin"),
            ForumId = post.Forum.Id,
            ForumName = post.Forum.Title,
            ForumImageUrl = post.Forum.ImageUrl,
            Created = DateTime.Now
        };
        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> CreateReply(PostReplyModel model)
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);
        var reply = await BuildReply(model, user);
        await _postService.AddReply(reply);
        await _userService.UpdateUserRating(userId, typeof(PostReply));
        return RedirectToAction("Index", "Post", new { id = model.PostId });
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var reply = await _replyService.GetById(id);
        if (reply == null) return RedirectToAction("Error", "Home");
        var model = new EditPostReplyModel
        {
            PostId = reply.Post.Id,
            ReplyId = reply.Id,
            AuthorName = reply.User.UserName,
            Content = reply.Content,
            PostTitle = reply.Post.Title
        };
        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditReply(EditPostReplyModel model)
    {
        await _replyService.Edit(model.ReplyId, model.Content);
        return RedirectToAction("Index", "Post", new { id = model.PostId });
    }

    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _replyService.Delete(id);
        var reply = await _replyService.GetById(id);
        return reply == null
            ? RedirectToAction("Error", "Home")
            : RedirectToAction("Index", "Post", new { id = reply.Post.Id });
    }

    private async Task<PostReply> BuildReply(PostReplyModel model, ApplicationUser user)
    {
        var post = await _postService.GetById(model.PostId);

        return new PostReply
        {
            Post = post,
            Content = model.ReplyContent,
            Created = DateTime.Now,
            User = user
        };
    }
}