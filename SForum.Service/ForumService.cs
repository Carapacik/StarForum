using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SForum.Data;
using SForum.Data.Models;

namespace SForum.Service
{
    public class ForumService : IForum
    {
        private readonly ApplicationDbContext _context;

        public ForumService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool HasRecentPost(int id)
        {
            const int hoursAgo = 12;
            var windows = DateTime.Now.AddHours(-hoursAgo);

            return GetById(id).Posts.Any(post => post.Created > windows);
        }

        public async Task Create(Forum forum)
        {
            _context.Add(forum);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int forumId)
        {
            var forum = GetById(forumId);
            _context.Remove(forum);
            await _context.SaveChangesAsync();
        }

        public async Task Edit(Forum forum)
        {
            var forumOld = GetById(forum.Id);
            forumOld.Description = forum.Description;
            forumOld.Title = forum.Title;
            if (forum.ImageUrl != "") forumOld.ImageUrl = forum.ImageUrl;

            _context.Forums.Update(forumOld);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Forum> GetAll()
        {
            return _context.Forums.Include(forum => forum.Posts);
        }

        public IEnumerable<ApplicationUser> GetActiveUsers(int id)
        {
            var posts = GetById(id).Posts;
            if (posts != null || !posts.Any())
            {
                var postUsers = posts.Select(p => p.User);
                var replyUsers = posts.SelectMany(r => r.Replies).Select(p => p.User);
                return postUsers.Union(replyUsers).Distinct();
            }

            return new List<ApplicationUser>();
        }

        public Forum GetById(int id)
        {
            var forum = _context.Forums.Where(f => f.Id == id)
                .Include(f => f.Posts).ThenInclude(f => f.User)
                .Include(f => f.Posts).ThenInclude(p => p.Replies).ThenInclude(r => r.User).FirstOrDefault();

            return forum;
        }

        public async Task UpdateForumDescription(int forumId, string newDescription)
        {
            var forum = GetById(forumId);
            forum.Description = newDescription;

            _context.Update(forum);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateForumTitle(int forumId, string newTitle)
        {
            var forum = GetById(forumId);
            forum.Title = newTitle;

            _context.Update(forum);
            await _context.SaveChangesAsync();
        }
    }
}