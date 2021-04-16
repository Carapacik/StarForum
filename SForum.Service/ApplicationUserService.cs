using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SForum.Data;
using SForum.Data.Models;

namespace SForum.Service
{
    public class ApplicationUserService : IApplicationUser
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(ApplicationUser user)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task Edit(ApplicationUser user)
        {
            var userOld = GetById(user.Id);
            if (user.NickName != null) userOld.NickName = user.NickName;
            if (user.UserDescription != null) userOld.UserDescription = user.UserDescription;
            if (!string.IsNullOrEmpty(user.ProfileImageUrl)) userOld.ProfileImageUrl = user.ProfileImageUrl;
            _context.ApplicationUsers.Update(userOld);
            await _context.SaveChangesAsync();
        }

        public async Task Deactivate(ApplicationUser user)
        {
            user.IsActive = false;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _context.ApplicationUsers;
        }

        public ApplicationUser GetById(string id)
        {
            return GetAll().FirstOrDefault(user => user.Id == id);
        }

        public async Task UpdateUserRating(string userId, Type type)
        {
            var user = GetById(userId);
            user.Rating = CalculateUserRating(type, user.Rating);
            await _context.SaveChangesAsync();
        }

        private static int CalculateUserRating(Type type, int userRating)
        {
            var inc = 0;
            if (type == typeof(Post)) inc = 1;
            if (type == typeof(PostReply)) inc = 3;
            return userRating + inc;
        }
    }
}