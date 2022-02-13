using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using StarForum.Infrastructure.Entities;

namespace StarForum.Infrastructure;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public DataSeeder(ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task SeedSuperUser()
    {
        var roleStore = new RoleStore<IdentityRole>(_context);
        var userStore = new UserStore<ApplicationUser>(_context);
        var user = new ApplicationUser
        {
            UserName = "ForumAdmin",
            NormalizedUserName = "FORUMADMIN",
            Email = "admin@starforum.com",
            NormalizedEmail = "ADMIN@STARFORUM.COM",
            EmailConfirmed = true,
            UserDescription = "ADMIN",
            IsAdmin = true,
            IsActive = true,
            LockoutEnabled = false,
            SecurityStamp = Guid.NewGuid().ToString(),
            MemberSince = DateTime.Today
        };
        var hasher = new PasswordHasher<ApplicationUser>();
        var hashedPassword = hasher.HashPassword(user, "admin");
        user.PasswordHash = hashedPassword;
        var hasAdminRole = _context.Roles.Any(r => r.Name == "Admin");
        if (!hasAdminRole) await roleStore.CreateAsync(new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" });
        var hasSuperUser = _context.Users.Any(u => u.NormalizedUserName == user.NormalizedUserName);
        if (!hasSuperUser)
        {
            await userStore.CreateAsync(user);
            await userStore.AddToRoleAsync(user, "ADMIN");
        }

        await _unitOfWork.Commit();
        await Task.CompletedTask;
    }
}