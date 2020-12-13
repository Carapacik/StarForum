using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SForum.Data;
using SForum.Data.Models;
using SForum.Models.ApplicationUser;

namespace SForum.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUpload _uploadService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _userService;

        public ProfileController(UserManager<ApplicationUser> userManager, IApplicationUser userService,
            IUpload uploadService)
        {
            _userManager = userManager;
            _userService = userService;
            _uploadService = uploadService;
        }

        public IActionResult Detail(string id)
        {
            var user = _userService.GetById(id);
            var userRoles = _userManager.GetRolesAsync(user).Result;
            var model = new ProfileModel
            {
                UserId = user.Id,
                Username = user.UserName,
                UserRating = user.Rating.ToString(),
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                MemberSince = user.MemberSince,
                IsAdmin = userRoles.Contains("Admin")
            };
            return View(model);
        }
    }
}