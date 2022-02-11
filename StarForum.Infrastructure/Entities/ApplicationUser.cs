using Microsoft.AspNetCore.Identity;

namespace StarForum.Infrastructure.Entities;

public class ApplicationUser : IdentityUser
{
    public string? UserDescription { get; set; }
    public string? ProfileImageUrl { get; set; }
    public int Rating { get; set; }
    public DateTime MemberSince { get; set; }
    public bool IsActive { get; set; }
    public bool IsAdmin { get; set; }
}