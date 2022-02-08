using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SForum.Data.Models;

namespace SForum.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;

        public DataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedSuperUser()
        {
            var roleStore = new RoleStore<IdentityRole>(_context);
            var userStore = new UserStore<ApplicationUser>(_context);
            var user = new ApplicationUser
            {
                UserName = "ForumAdmin",
                NormalizedUserName = "FORUMADMIN",
                Email = "admin@sforum.com",
                NormalizedEmail = "ADMIN@SFORUM.COM",
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
            if (!hasAdminRole) await roleStore.CreateAsync(new IdentityRole {Name = "Admin", NormalizedName = "ADMIN"});
            var hasSuperUser = _context.Users.Any(u => u.NormalizedUserName == user.NormalizedUserName);
            if (!hasSuperUser)
            {
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, "ADMIN");
            }

            await _context.SaveChangesAsync();
            await Task.CompletedTask;
        }
    }
}