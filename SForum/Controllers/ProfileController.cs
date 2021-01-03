using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using SForum.Data;
using SForum.Data.Models;
using SForum.Models.ApplicationUser;

namespace SForum.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUpload _uploadService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(UserManager<ApplicationUser> userManager, IApplicationUser userService,
            IUpload uploadService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _userService = userService;
            _uploadService = uploadService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index(string typeQuery = "rating", string oldTypeQuery = "")
        {
            IEnumerable<ProfileModel> profiles;
            switch (typeQuery)
            {
                case "memberSince" when oldTypeQuery == typeQuery:
                    profiles = _userService.GetAll().OrderBy(user => user.MemberSince).Select(u =>
                        new ProfileModel
                        {
                            UserId = u.Id,
                            Email = u.Email,
                            Username = u.UserName,
                            ProfileImageUrl = u.ProfileImageUrl,
                            UserRating = u.Rating.ToString(),
                            MemberSince = u.MemberSince
                        });
                    oldTypeQuery = "0";
                    break;
                case "memberSince":
                    profiles = _userService.GetAll().OrderByDescending(user => user.MemberSince).Select(u =>
                        new ProfileModel
                        {
                            UserId = u.Id,
                            Email = u.Email,
                            Username = u.UserName,
                            ProfileImageUrl = u.ProfileImageUrl,
                            UserRating = u.Rating.ToString(),
                            MemberSince = u.MemberSince
                        });
                    oldTypeQuery = typeQuery;
                    break;
                case "userName" when oldTypeQuery == typeQuery:
                    profiles = _userService.GetAll().OrderByDescending(user => user.NormalizedUserName).Select(u =>
                        new ProfileModel
                        {
                            UserId = u.Id,
                            Email = u.Email,
                            Username = u.UserName,
                            ProfileImageUrl = u.ProfileImageUrl,
                            UserRating = u.Rating.ToString(),
                            MemberSince = u.MemberSince
                        });
                    oldTypeQuery = "0";
                    break;
                case "userName":
                    profiles = _userService.GetAll().OrderBy(user => user.NormalizedUserName).Select(u =>
                        new ProfileModel
                        {
                            UserId = u.Id,
                            Email = u.Email,
                            Username = u.UserName,
                            ProfileImageUrl = u.ProfileImageUrl,
                            UserRating = u.Rating.ToString(),
                            MemberSince = u.MemberSince
                        });
                    oldTypeQuery = typeQuery;
                    break;
                case "email" when oldTypeQuery == typeQuery:
                    profiles = _userService.GetAll().OrderByDescending(user => user.Email).Select(u =>
                        new ProfileModel
                        {
                            UserId = u.Id,
                            Email = u.Email,
                            Username = u.UserName,
                            ProfileImageUrl = u.ProfileImageUrl,
                            UserRating = u.Rating.ToString(),
                            MemberSince = u.MemberSince
                        });
                    oldTypeQuery = "0";
                    break;
                case "email":
                    profiles = _userService.GetAll().OrderBy(user => user.Email).Select(u =>
                        new ProfileModel
                        {
                            UserId = u.Id,
                            Email = u.Email,
                            Username = u.UserName,
                            ProfileImageUrl = u.ProfileImageUrl,
                            UserRating = u.Rating.ToString(),
                            MemberSince = u.MemberSince
                        });
                    oldTypeQuery = typeQuery;
                    break;
                default:
                {
                    if (oldTypeQuery == typeQuery)
                    {
                        profiles = _userService.GetAll().OrderBy(user => user.Rating).Select(u =>
                            new ProfileModel
                            {
                                UserId = u.Id,
                                Email = u.Email,
                                Username = u.UserName,
                                ProfileImageUrl = u.ProfileImageUrl,
                                UserRating = u.Rating.ToString(),
                                MemberSince = u.MemberSince
                            });
                        oldTypeQuery = "0";
                    }
                    else
                    {
                        profiles = _userService.GetAll().OrderByDescending(user => user.Rating).Select(u =>
                            new ProfileModel
                            {
                                UserId = u.Id,
                                Email = u.Email,
                                Username = u.UserName,
                                ProfileImageUrl = u.ProfileImageUrl,
                                UserRating = u.Rating.ToString(),
                                MemberSince = u.MemberSince
                            });
                        oldTypeQuery = typeQuery;
                    }

                    break;
                }
            }

            var model = new ProfileListModel
            {
                Profiles = profiles,
                TypeQuery = typeQuery,
                OldTypeQuery = oldTypeQuery
            };
            return View(model);
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
                UserDescription = user.UserDescription,
                IsAdmin = userRoles.Contains("Admin")
            };
            return View(model);
        }

        [Authorize]
        public IActionResult Edit(string id)
        {
            var user = _userService.GetById(id);
            var userRoles = _userManager.GetRolesAsync(user).Result;
            var model = new ProfileModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                UserDescription = user.UserDescription,
                ProfileImageUrl = user.ProfileImageUrl,
                IsAdmin = userRoles.Contains("Admin"),
                MemberSince = user.MemberSince
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileModel model)
        {
            var userId = _userManager.GetUserId(User);
            var imageUri = "";
            if (model.ImageUpload != null)
            {
                //imageUri = UploadProfileImage(model.ImageUpload); без Azure
                var blockBlob = UploadProfileImageForAzure(model.ImageUpload);
                imageUri = blockBlob.Uri.AbsoluteUri;
            }

            var forum = new ApplicationUser
            {
                Id = model.UserId,
                UserDescription = model.UserDescription,
                UserName = model.Username,
                ProfileImageUrl = imageUri
            };

            await _userService.Edit(forum);
            return RedirectToAction("Detail", "Profile", new {id = userId});
        }

        private CloudBlockBlob UploadProfileImageForAzure(IFormFile file)
        {
            if (file.Length > 4 * 1024 * 1024 && !file.ContentType.Contains("image"))
                return null;
            var connectionString = _configuration.GetConnectionString("AzureStorageAccount");
            var container = _uploadService.GetBlobContainer(connectionString, "profile-images");
            var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            if (contentDisposition.FileName == null) return null;
            var filename = contentDisposition.FileName.Trim('"');
            var blockBlob =
                container.GetBlockBlobReference(Guid.NewGuid() + Path.GetExtension(filename));
            blockBlob.UploadFromStreamAsync(file.OpenReadStream()).Wait();

            return blockBlob;
        }

        private string UploadProfileImage(IFormFile file)
        {
            if (file.Length > 4 * 1024 * 1024 && !file.ContentType.Contains("image")) return null;
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            if (contentDisposition.FileName == null) return null;
            var filename = contentDisposition.FileName.Trim('"');
            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(filename);
            var path = Path.Combine(wwwRootPath + "/images/profile-images/", uniqueFileName);
            using var fileStream = new FileStream(path, FileMode.Create);
            file.CopyTo(fileStream);

            return path;
        }
    }
}