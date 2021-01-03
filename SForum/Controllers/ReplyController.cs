using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SForum.Data;
using SForum.Data.Models;
using SForum.Models.Reply;

namespace SForum.Controllers
{
    [Authorize]
    public class ReplyController : Controller
    {
        private readonly IPostReply _replyService;
        private readonly IPost _postService;
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
            var post = _postService.GetById(id);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
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
        public async Task<ActionResult> AddReply(PostReplyModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var reply = BuildReply(model, user);
            await _postService.AddReply(reply);
            await _userService.UpdateUserRating(userId, typeof(PostReply));
            return RedirectToAction("Index", "Post", new {id = model.PostId});
        }

        private PostReply BuildReply(PostReplyModel model, ApplicationUser user)
        {
            var post = _postService.GetById(model.PostId);
            return new PostReply
            {
                Post = post,
                Content = model.ReplyContent,
                Created = DateTime.Now,
                User = user
            };
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            var reply = _replyService.GetById(id);
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

    }
}