using System;
using Microsoft.AspNetCore.Http;

namespace SForum.Models.ApplicationUser
{
    public class ProfileModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public string UserName { get; set; }
        public string UserRating { get; set; }
        public string ProfileImageUrl { get; set; }
        public string UserDescription { get; set; }
        public DateTime MemberSince { get; set; }
        public IFormFile ImageUpload { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
    }
}