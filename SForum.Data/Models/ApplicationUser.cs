using System;
using Microsoft.AspNetCore.Identity;

namespace SForum.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Rating { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public string NickName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string UserDescription { get; set; }
        public DateTime MemberSince { get; set; }
    }
}