using System;
using Microsoft.AspNetCore.Identity;

namespace SForum.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Rating { get; set; }
        public string ProfileImage { get; set; }
        public DateTime MemberSicne { get; set; }
        public bool IsActive { get; set; }
    }
}