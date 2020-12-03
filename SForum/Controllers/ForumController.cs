using Microsoft.AspNetCore.Mvc;
using SForum.Data;
using SForum.Models.Forum;
using System.Linq;

namespace SForum.Controllers
{
    public class ForumController : Controller
    {
        private readonly IForum _forumService;

        public ForumController(IForum forumService)
        {
            _forumService = forumService;
        }
        public IActionResult Index()
        {
            var forums = _forumService.GetAll()
                .Select(forum => new ForumListingModel
            {
                Id = forum.Id, 
                Name = forum.Title, 
                Description = forum.Description
            });

            var model = new ForumIndexModel
            {
                ForumList = forums
        };
            return View(model);
        }
    }
}
